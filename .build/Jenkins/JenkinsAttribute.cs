using Jenkins.Configuration;
using Jenkins.Utils;
using Nuke.Common;
using Nuke.Common.CI;
using Nuke.Common.Execution;
using Nuke.Common.IO;
using Nuke.Common.Utilities;
using Nuke.Common.Utilities.Collections;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Jenkins
{
    public class JenkinsAttribute : ChainedConfigurationAttributeBase
    {
        private AbsolutePath TeamcityDirectory => NukeBuild.RootDirectory / ".jenkins";
        private string JenkinsFile => TeamcityDirectory / "Jenkinsfile";

        public string[] InvokedTargets { get; set; } = new string[0];

        public override HostType HostType => HostType.Jenkins;

        public override IEnumerable<string> GeneratedFiles => new[] { JenkinsFile };

        public override IEnumerable<string> RelevantTargetNames => InvokedTargets;

        public override CustomFileWriter CreateWriter()
        {
            return new CustomFileWriter(JenkinsFile, indentationFactor: 2, commentPrefix: "//");
        }

        public override ConfigurationEntity GetConfiguration(
            NukeBuild build,
            IReadOnlyCollection<ExecutableTarget> relevantTargets)
        {
            return new JenkinsConfiguration
            {
                Stages = GetStages(relevantTargets).ToArray(),
            };
        }
        
        protected virtual IEnumerable<JenkinsPipelineStage> GetStages(IReadOnlyCollection<ExecutableTarget> relevantTargets)
        {
            return relevantTargets.Select(t => GetStage(t))
            .Prepend(new JenkinsPipelineStage
            {
                Name = "Checkout",
                IsCheckoutScm = true,
                Agent = new JenkinsPipelineAgent(),
                Stashes = new []
                {
                    new JenkinsPipelineStash
                    {
                        Name = "source",
                        Includes = "**/*",
                    }
                }
            });
        }

        protected virtual JenkinsPipelineStage GetStage(ExecutableTarget executableTarget)
        {
            IEnumerable<string> artifactProduct = executableTarget.GetArtifactProducts().ToArray();
            IEnumerable<JenkinsPipelineStash> stashes = artifactProduct
                .Select((v, i) => GetStash(executableTarget, v))
                .OrderBy(s => s.Name);

            IEnumerable<(Target, string[])> artifactDependencies = ArtifactExtensionsAccessor.ArtifactDependencies[executableTarget.GetDefinition()].ToArray();
            IEnumerable<JenkinsPipelineUnstash> unstashes = artifactDependencies
                .SelectMany(d => GetUnstashesFromDependency(executableTarget, d.Item1, d.Item2))
                .OrderBy(s => s.Name);;

            return new JenkinsPipelineStage
            {
                Name = executableTarget.Name,
                InvokedTarget = executableTarget.Name,
                Agent = new JenkinsPipelineAgent(),
                Stashes = stashes.ToArray(),
                Unstashes = unstashes.ToArray(),
            };
        }

        protected virtual JenkinsPipelineStash GetStash(ExecutableTarget executableTarget, string includes)
        {
            return new JenkinsPipelineStash
            {
                Name = $"{executableTarget.Name.ToLowerInvariant()}_{includes.GetHashCode():X}",
                Includes = includes,
            };
        }

        protected virtual IEnumerable<JenkinsPipelineUnstash> GetUnstashesFromDependency(
            ExecutableTarget executableTarget,
            Target dependency,
            string[] artifacts)
        {
            IEnumerable<string> consumedArtifacts = artifacts
                .Select(a => $"{executableTarget.Name.ToLowerInvariant()}_{a.GetHashCode():X}")
                .ToArray();

            IEnumerable<string> producedArtifacts = executableTarget
                .ResolveExecutionDependency(dependency)
                .GetArtifactProducts()
                .Select(a => $"{executableTarget.Name.ToLowerInvariant()}_{a.GetHashCode():X}")
                .ToArray();

            if (!consumedArtifacts.Any())
            {
                consumedArtifacts = producedArtifacts;
            }

            return consumedArtifacts.Intersect(producedArtifacts)
                .Select(a => new JenkinsPipelineUnstash
                {
                    Name = a,
                });
        }
    }
}