using Nuke.Common;
using Nuke.Common.IO;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tools.DotNet;
using Utils;
using static Nuke.Common.Tools.DotNet.DotNetTasks;
using static Nuke.Common.EnvironmentInfo;
using static Nuke.Common.Logger;
using static Nuke.Common.Tooling.ProcessTasks;

public partial class Build
{
        Target Database_Upgrade=> _ => _
        .Executes(() =>
        {
            Project project = Solution.GetProject("Demo.Tools.DatabaseMigrator");
            if (project == null)
            {
                Error("The project Demo.Tools.DatabaseMigrator cannot be found.");
                return;
            }

            var connectionString = "Server=localhost;Database=Demo;User Id=SA;Password=SQLSecure?;";
            AbsolutePath scriptsDirectory = RootDirectory / "deployment/demo.database/scripts";

            DotNetBuild(_ => _
                .SetProjectFile(project));

            DotNetRun(_ => _
                .SetProjectFile(project)
                .SetApplicationArguments($"create -c \"{connectionString}\"")
                .EnableNoRestore()
                .EnableNoBuild());

            DotNetRun(_ => _
                .SetProjectFile(project)
                .SetApplicationArguments($"up -c \"{connectionString}\" -s \"{scriptsDirectory}\"")
                .EnableNoRestore()
                .EnableNoBuild());
        });
    
    Target Database_Upgrade_Test => _ => _
        .Executes(() =>
        {
            Project project = Solution.GetProject("Demo.Tools.DatabaseMigrator");
            if (project == null)
            {
                Error("The project Demo.Tools.DatabaseMigrator cannot be found.");
                return;
            }

            var connectionString = "Server=localhost;Database=Demo.Test;User Id=SA;Password=SQLSecure?;";
            AbsolutePath scriptsDirectory = RootDirectory / "deployment/demo.database/scripts";

            DotNetBuild(_ => _
                .SetProjectFile(project));

            DotNetRun(_ => _
                .SetProjectFile(project)
                .SetApplicationArguments($"create -c \"{connectionString}\"")
                .EnableNoRestore()
                .EnableNoBuild());

            DotNetRun(_ => _
                .SetProjectFile(project)
                .SetApplicationArguments($"up -c \"{connectionString}\" -s \"{scriptsDirectory}\"")
                .EnableNoRestore()
                .EnableNoBuild());
        });

    Target Database_Prepare => _ => _
        .DependsOn(Database_Upgrade)
        .DependsOn(Database_Upgrade_Test)
        .Executes(() =>
        {
        });
    
    Target DockerRunDemo=> _ => _
        .DependsOn(BuildDockerImage)
        .Executes(() =>
        {
            SetVariable("DOCKER_TAG", GitVersion.GetDockerTag());

            StartProcess(
                "docker-compose",
                "-f docker-compose.yml up",
                SourceDirectory,
                Variables,
                null,
                true,
                true).WaitForExit();
        });

    Target DockerRunDevEnv=> _ => _
        .DependsOn(BuildDockerImage)
        .Executes(() =>
        {
            SetVariable("DOCKER_TAG", GitVersion.GetDockerTag());

            StartProcess(
                "docker-compose",
                "-f docker-compose.dev-env.yml up -d",
                SourceDirectory,
                Variables,
                null,
                true,
                true).WaitForExit();
        });
}
