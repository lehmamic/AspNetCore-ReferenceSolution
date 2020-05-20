using Jenkins.Configuration;
using Jenkins.Utils;
using Nuke.Common;
using Nuke.Common.CI;
using Nuke.Common.Execution;
using Nuke.Common.IO;
using Nuke.Common.Utilities;
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
            IEnumerable<string> artifactProduct = ArtifactExtensionsAccessor.ArtifactProducts[executableTarget.GetDefinition()].ToArray();
            IEnumerable<JenkinsPipelineStash> stashes = artifactProduct.Select((v, i) =>
                GetStash(executableTarget, v, artifactProduct.Count() > 1 ? $"{i}" : string.Empty));
Console.WriteLine($"{executableTarget.Name}: produces: {string.Join(", ", ArtifactExtensionsAccessor.ArtifactProducts.Count)}");
            return new JenkinsPipelineStage
            {
                Name = executableTarget.Name,
                InvokedTarget = executableTarget.Name,
                Agent = new JenkinsPipelineAgent(),
                Stashes = stashes.ToArray(),
            };
        }

        protected virtual JenkinsPipelineStash GetStash(ExecutableTarget executableTarget, string includes, string postfix)
        {
            string fullPostfix = string.IsNullOrWhiteSpace(postfix) ? string.Empty : $"_{postfix}";
            return new JenkinsPipelineStash
            {
                Name = $"{executableTarget.Name.ToLowerInvariant()}{fullPostfix}",
                Includes = includes,
            };
        }
    }
}