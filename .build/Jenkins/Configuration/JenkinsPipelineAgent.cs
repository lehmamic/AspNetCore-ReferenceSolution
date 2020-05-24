using Jenkins.Utils;
using Nuke.Common.CI;
using Nuke.Common.Utilities;

namespace Jenkins.Configuration
{
    public class JenkinsPipelineAgent : ConfigurationEntity
    {
        public JenkinsAgentType AgentType { get; set; }
        
        public JenkinsAgentPlatform AgentPlatform { get; set; }

        public string Label { get; set; }

        public string CustomWorkspace { get; set; }

        public string DockerImage { get; set; }

        public string DockerArgs { get; set; }
        
        public string DockerRegistryUrl { get; set; }

        public string DockerRegistryCredentialsId { get; set; }

        public string DockerFileName { get; set; }

        public string DockerContextDirectory { get; set; }

        public string DockerAdditionalBuildArgs { get; set; }

        public string KubernetesYaml { get; set; }

        public string KubernetesYamlFile { get; set; }

        public string KubernetesDefaultContainer { get; set; }

        public override void Write(CustomFileWriter writer)
        {
            if (AgentType == JenkinsAgentType.None || AgentType == JenkinsAgentType.Any)
            {
                writer.WriteLine($"agent {AgentType.ToString().ToLowerInvariant()}");
            }
            else
            {
                using (writer.WriteBlock($"agent"))
                {
                    using (writer.WriteBlock($"{AgentType.ToString().ToLowerInvariant()}"))
                    {
                        if (!string.IsNullOrWhiteSpace(Label))
                        {
                            writer.WriteLine($"label '{Label}'");
                        }

                        if (!string.IsNullOrWhiteSpace(CustomWorkspace))
                        {
                            writer.WriteLine($"customWorkspace '{CustomWorkspace}'");
                        }

                        if (AgentType == JenkinsAgentType.Docker)
                        {
                            if (!string.IsNullOrWhiteSpace(DockerImage))
                            {
                                writer.WriteLine($"image '{DockerImage}'");
                            }

                            if (!string.IsNullOrWhiteSpace(DockerArgs))
                            {
                                writer.WriteLine($"args '{DockerArgs}'");
                            }
                            
                            if (!string.IsNullOrWhiteSpace(DockerRegistryUrl))
                            {
                                writer.WriteLine($"registryUrl '{DockerRegistryUrl}'");
                            }

                            if (!string.IsNullOrWhiteSpace(DockerRegistryCredentialsId))
                            {
                                writer.WriteLine($"registryCredentialsId '{DockerRegistryCredentialsId}'");
                            }
                        }

                        if (AgentType == JenkinsAgentType.DockerFile)
                        {
                            if (!string.IsNullOrWhiteSpace(DockerFileName))
                            {
                                writer.WriteLine($"filename '{DockerFileName}'");
                            }

                            if (!string.IsNullOrWhiteSpace(DockerContextDirectory))
                            {
                                writer.WriteLine($"dir '{DockerContextDirectory}'");
                            }

                            if (!string.IsNullOrWhiteSpace(DockerAdditionalBuildArgs))
                            {
                                writer.WriteLine($"additionalBuildArgs '{DockerAdditionalBuildArgs}'");
                            }

                            if (!string.IsNullOrWhiteSpace(DockerArgs))
                            {
                                writer.WriteLine($"args '{DockerArgs}'");
                            }
                            
                            if (!string.IsNullOrWhiteSpace(DockerRegistryUrl))
                            {
                                writer.WriteLine($"registryUrl '{DockerRegistryUrl}'");
                            }

                            if (!string.IsNullOrWhiteSpace(DockerRegistryCredentialsId))
                            {
                                writer.WriteLine($"registryCredentialsId '{DockerRegistryCredentialsId}'");
                            }
                        }

                        if (AgentType == JenkinsAgentType.Kubernetes)
                        {
                            if (!string.IsNullOrWhiteSpace(KubernetesYaml))
                            {
                                writer.WriteLine($"yaml \"\"\"{KubernetesYaml}\"\"\"");
                            }

                            if (!string.IsNullOrWhiteSpace(KubernetesYamlFile))
                            {
                                writer.WriteLine($"yamlFile '{KubernetesYamlFile}'");
                            }

                            if (!string.IsNullOrWhiteSpace(KubernetesDefaultContainer))
                            {
                                writer.WriteLine($"defaultContainer '{KubernetesDefaultContainer}'");
                            }
                        }
                    }
                }
            }
        }
    }
}