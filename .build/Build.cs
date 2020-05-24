using Jenkins;
using Jenkins.Utils;
using Nuke.Common;
using Nuke.Common.CI;
using Nuke.Common.Execution;
using Nuke.Common.Git;
using Nuke.Common.IO;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tooling;
using Nuke.Common.Tools.DotNet;
using Nuke.Common.Tools.GitVersion;
using Nuke.Common.Tools.Npm;
using Nuke.Common.Utilities.Collections;
using Nuke.Docker;
using Utils;
using static Nuke.Common.IO.FileSystemTasks;
using static Nuke.Common.Logger;
using static Nuke.Common.Tools.DotNet.DotNetTasks;
using static Nuke.Common.Tools.Npm.NpmTasks;
using static Nuke.Docker.DockerTasks;

[JenkinsPipeline(
    InvokedTargets = new []
    {
        nameof(BuildDockerImage),
    }
)]
[JenkinsAgent(
    JenkinsAgentType.Kubernetes,
    JenkinsAgentPlatform.Unix,
    Label = "dotnet",
    KubernetesYamlFile = ".jenkins/pod-template.yaml",
    KubernetesDefaultContainer = "dotnet"
)]
[JenkinsAgent(
    JenkinsAgentType.Node,
    JenkinsAgentPlatform.Windows,
    Label = "node"
)]
[CheckBuildProjectConfigurations]
[UnsetVisualStudioEnvironmentVariables]
public partial class Build : NukeBuild
{
    /// Support plugins are available for:
    ///   - JetBrains ReSharper        https://nuke.build/resharper
    ///   - JetBrains Rider            https://nuke.build/rider
    ///   - Microsoft VisualStudio     https://nuke.build/visualstudio
    ///   - Microsoft VSCode           https://nuke.build/vscode

    public static int Main () => Execute<Build>(x => x.BackendCompile);

    [Parameter("Configuration to build - Default is 'Debug' (local) or 'Release' (server)")]
    readonly Configuration Configuration = IsLocalBuild ? Configuration.Debug : Configuration.Release;

    [Solution] readonly Solution Solution;
    [GitRepository] readonly GitRepository GitRepository;
    [GitVersion] readonly GitVersion GitVersion;

    AbsolutePath SourceDirectory => RootDirectory / "src";
    AbsolutePath TestsDirectory => RootDirectory / "tests";
    AbsolutePath ArtifactsDirectory => RootDirectory / "artifacts";

    Target Clean => _ => _
        .Before(BackendRestore)
        .Executes(() =>
        {
            SourceDirectory.GlobDirectories("**/bin", "**/obj", "**/dist").ForEach(DeleteDirectory);
            TestsDirectory.GlobDirectories("**/bin", "**/obj").ForEach(DeleteDirectory);
            EnsureCleanDirectory(ArtifactsDirectory);
        });

    Target BackendRestore => _ => _
        .RunsOnAgent("dotnet")
        .Executes(() =>
        {
            DotNetRestore(s => s
                .SetProjectFile(Solution));
        });

    Target BackendCompile => _ => _
        .DependsOn(BackendRestore)
        .Produces("**/bin/**/*", "**/obj/**/*")
        .RunsOnAgent("dotnet")
        .Executes(() =>
        {
            DotNetBuild(s => s
                .SetProjectFile(Solution)
                .SetConfiguration(Configuration)
                .SetAssemblyVersion(GitVersion.AssemblySemVer)
                .SetFileVersion(GitVersion.AssemblySemFileVer)
                .SetInformationalVersion(GitVersion.InformationalVersion)
                .EnableNoRestore());
        });

    Target BackendTest => _ => _
        .DependsOn(BackendCompile)
        .Consumes(BackendCompile)
        .RunsOnAgent("dotnet")
        .Executes(() =>
        {
            DotNetTest(_ => _
                .SetProjectFile(Solution)
                .SetConfiguration(Configuration)
                .EnableNoRestore()
                .EnableNoBuild()
            );
        });

    Target BackendPublish => _ => _
        .DependsOn(BackendTest, BackendCompile)
        .Consumes(BackendCompile)
        .RunsOnAgent("dotnet")
        .Executes(() =>
        {
            Project project = Solution.GetProject("Demo.Backend");
            if (project == null)
            {
                Error("The project Demo.Backend cannot be found.");
                return;
            }

            DotNetPublish(_ => _
                .SetProject(project)
                .SetConfiguration(Configuration)
                .SetAssemblyVersion(GitVersion.AssemblySemVer)
                .SetFileVersion(GitVersion.AssemblySemFileVer)
                .SetInformationalVersion(GitVersion.InformationalVersion)
                .SetOutput(ArtifactsDirectory / "publish" / "demo" / "demo.backend"));
        });

    Target FrontendInstall => _ => _
        .RunsOnAgent("node")
        .Executes(() =>
        {
            NpmInstall(_ => _
                .SetWorkingDirectory(SourceDirectory / "Demo.Frontend")
                .DisableLogOutput());
        });

    Target FrontendBuild => _ => _
        .DependsOn(FrontendInstall)
        .RunsOnAgent("node")
        .Executes(() =>
        {
            NpmRun(_ => _
                .SetWorkingDirectory(SourceDirectory / "Demo.Frontend")
                .SetCommand("lint"));

            NpmRun(_ => _
                .SetWorkingDirectory(SourceDirectory / "Demo.Frontend")
                .SetCommand("build")
                .SetArguments("--prod"));
        });

    Target FrontendTest => _ => _
        .DependsOn(FrontendBuild)
        .RunsOnAgent("node")
        .Executes(() =>
        {
            NpmRun(_ => _
                .SetWorkingDirectory(SourceDirectory / "Demo.Frontend")
                .SetCommand("test")
                .SetArguments("--watch=false", "--browsers=ChromeHeadless", IsLocalBuild ? "" : "--reporters=teamcity"));
        });
    
    Target FrontendPublish => _ => _
        .DependsOn(FrontendTest)
        .RunsOnAgent("node")
        .Executes(() =>
        {
            CopyDirectoryRecursively(SourceDirectory / "Demo.Frontend" / "dist" / "demo-frontend", ArtifactsDirectory / "publish" / "demo" / "demo.frontend", DirectoryExistsPolicy.Merge, FileExistsPolicy.Overwrite);
        });

    Target BuildDockerImage => _ => _
        .DependsOn(BackendPublish, FrontendPublish)
        .RunsOnAgent("docker")
        .Executes(() =>
        {
            Project project = Solution.GetProject("Demo.Backend");
            if (project == null)
            {
                Error("The project Demo.Backend cannot be found.");
                return;
            }

            // the dockerfile must be within the context folder
            CopyFile(project.Directory / "Dockerfile", ArtifactsDirectory / "publish" / "demo" / "Dockerfile", FileExistsPolicy.Overwrite);

            DockerBuild(_ => _
                .SetFile(ArtifactsDirectory / "publish" / "demo" / "Dockerfile")
                .SetPath(ArtifactsDirectory)
                .SetTag($"demo:{GitVersion.GetDockerTag()}"));
        });
}
