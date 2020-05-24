using Jenkins.Configuration;
using Jenkins.Utils;
using Nuke.Common;
using Nuke.Common.CI;
using Nuke.Common.Execution;
using Nuke.Common.IO;
using Nuke.Common.Tooling;
using Nuke.Common.Utilities;
using Nuke.Common.Utilities.Collections;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;

namespace Jenkins
{
    public class JenkinsPipelineAttribute : ChainedConfigurationAttributeBase
    {
        private AbsolutePath TeamcityDirectory => NukeBuild.RootDirectory / ".jenkins";
        private string JenkinsFile => TeamcityDirectory / "Jenkinsfile";

        public string[] InvokedTargets { get; set; } = new string[0];

        public string CronTrigger { get; set; }

        public string PollTrigger { get; set; } = "* * * * *";

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
                Agent = new JenkinsPipelineAgent
                {
                    AgentType = JenkinsAgentType.None,
                },
                Stages = GetStages(relevantTargets).ToArray(),
                Triggers = new JenkinsPipelineTriggers
                {
                    CronTrigger = CronTrigger,
                    PollTrigger = PollTrigger,
                },
            };
        }

        protected virtual IEnumerable<IJenkinsPipelineStage> GetStages(IReadOnlyCollection<ExecutableTarget> relevantTargets)
        {
            IReadOnlyDictionary<string, JenkinsPipelineAgent> registeredAgents = GetRegisteredJenkinsAgents(relevantTargets);

            var groupedTargets = new LookupTable<int, ExecutableTarget>();
            GroupByDependencies(groupedTargets, relevantTargets, relevantTargets, 0);

            return groupedTargets.Select(g => GetGroupedStages(g, registeredAgents))
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

        protected virtual IReadOnlyDictionary<string, JenkinsPipelineAgent> GetRegisteredJenkinsAgents(
            IReadOnlyCollection<ExecutableTarget> relevantTargets)
        {
            var result = new Dictionary<string, JenkinsPipelineAgent>();
            ExecutableTarget target = relevantTargets.FirstOrDefault();
            if (target != null && target.Member != null && target.Member.DeclaringType != null)
            {
                Dictionary<string, JenkinsPipelineAgent> agents = target.Member.DeclaringType
                    .GetCustomAttributes<JenkinsAgentAttribute>()
                    .Select(GetRegisteredJenkinsAgent)
                    .ToDictionary(a => a.Label, a => a);

                result.AddDictionary(agents);
            }

            return new ReadOnlyDictionary<string, JenkinsPipelineAgent>(result);
        }

        protected virtual JenkinsPipelineAgent GetRegisteredJenkinsAgent(JenkinsAgentAttribute attribute)
        {
            return new JenkinsPipelineAgent
            {
                AgentType = attribute.AgentType,
                AgentPlatform = attribute.AgentPlatform,
                Label = attribute.Label,
                CustomWorkspace = attribute.CustomWorkspace,
                DockerImage = attribute.DockerImage,
                DockerArgs = attribute.DockerArgs,
                DockerRegistryUrl = attribute.DockerRegistryUrl,
                DockerRegistryCredentialsId = attribute.DockerRegistryCredentialsId,
                DockerFileName = attribute.DockerFileName,
                DockerContextDirectory = attribute.DockerContextDirectory,
                DockerAdditionalBuildArgs = attribute.DockerAdditionalBuildArgs,
                KubernetesYaml = attribute.KubernetesYaml,
                KubernetesYamlFile = attribute.KubernetesYamlFile,
                KubernetesDefaultContainer = attribute.KubernetesDefaultContainer,
            };
        }

        protected virtual void GroupByDependencies(
            LookupTable<int, ExecutableTarget> groupedTargets,
            IEnumerable<ExecutableTarget> unorderedTargets,
            IReadOnlyCollection<ExecutableTarget> allTargets,
            int slot)
        {
            var remainingTargets = new List<ExecutableTarget>();
            foreach (ExecutableTarget target in unorderedTargets)
            {
                IEnumerable<ExecutableTarget> matchedDependencies = GetMatchedDependencies(groupedTargets, allTargets, target, slot);
                IEnumerable<ExecutableTarget> allDependencies = target.ExecutionDependencies
                    .Union(target.AllDependencies.Intersect(allTargets));

                if (allDependencies.Count() == matchedDependencies.Count())
                {
                    // add the target to the slot if all dependencies are matched
                    groupedTargets.Add(slot, target);
                }
                else
                {
                    remainingTargets.Add(target);
                }
            }

            if (remainingTargets.Any())
            {
                GroupByDependencies(groupedTargets, remainingTargets,  allTargets, slot + 1);
            }
        }

        protected virtual IEnumerable<ExecutableTarget> GetMatchedDependencies(
            LookupTable<int, ExecutableTarget> groupedTargets,
            IReadOnlyCollection<ExecutableTarget> allTargets,
            ExecutableTarget target,
            int slot)
        {
            var matchedDependencies = new List<ExecutableTarget>();
            for (var i = 0; i < slot; i++)
            {
                IEnumerable<ExecutableTarget> allDependencies = target.ExecutionDependencies
                    .Union(target.AllDependencies.Intersect(allTargets));

                foreach (ExecutableTarget dependency in allDependencies)
                {
                    if (groupedTargets[i].Select(t => t.Name).Contains(dependency.Name))
                    {
                        matchedDependencies.Add(dependency);
                    }
                }
            }

            return matchedDependencies;
        }

        protected virtual IJenkinsPipelineStage GetGroupedStages(IEnumerable<ExecutableTarget> executableTargets, IReadOnlyDictionary<string, JenkinsPipelineAgent> registeredAgents)
        {
            IJenkinsPipelineStage[] stages = executableTargets.Select(t => GetStage(t, registeredAgents))
                .OrderBy(s => s.Name)
                .ToArray();

            if (stages.Length > 1)
            {
                return new JenkinsPipelineParallelStage
                {
                    Name = String.Empty,
                    Stages = stages,
                };
            }

            return stages.First();
        }

        protected virtual JenkinsPipelineStage GetStage(ExecutableTarget executableTarget, IReadOnlyDictionary<string, JenkinsPipelineAgent> registeredAgents)
        {
            IEnumerable<string> artifactProduct = executableTarget.GetArtifactProducts().ToArray();
            IEnumerable<JenkinsPipelineStash> stashes = artifactProduct
                .Select((v, i) => GetStash(executableTarget, v))
                .OrderBy(s => s.Name);

            IEnumerable<(Target, string[])> artifactDependencies = ArtifactExtensionsAccessor.ArtifactDependencies[executableTarget.GetDefinition()].ToArray();
            IEnumerable<JenkinsPipelineUnstash> unstashes = artifactDependencies
                .SelectMany(d => GetUnstashesFromDependency(executableTarget, d.Item1, d.Item2))
                .OrderBy(s => s.Name)
                .Prepend(new JenkinsPipelineUnstash
                {
                    Name = "source",
                });

            JenkinsPipelineAgent agent = GetAgent(executableTarget, registeredAgents);

            return new JenkinsPipelineStage
            {
                Name = executableTarget.Name,
                InvokedTarget = executableTarget.Name,
                Agent = agent,
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
        
        protected virtual JenkinsPipelineAgent GetAgent(ExecutableTarget executableTarget, IReadOnlyDictionary<string, JenkinsPipelineAgent> registeredAgents)
        {
            JenkinsPipelineAgent agent;
            string agentLabel = executableTarget.GetAgents().FirstOrDefault();
            if (string.IsNullOrWhiteSpace(agentLabel))
            {
                agent = new JenkinsPipelineAgent
                {
                    AgentType = JenkinsAgentType.Any,
                    AgentPlatform = JenkinsAgentPlatform.Unix,
                };
            }
            else if (!registeredAgents.TryGetValue(agentLabel, out agent))
            {
                agent = new JenkinsPipelineAgent
                {
                    AgentType = JenkinsAgentType.Node,
                    AgentPlatform = JenkinsAgentPlatform.Unix,
                    Label = agentLabel,
                };
            }

            return agent;
        }
    }
}