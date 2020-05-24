using System;

namespace Jenkins.Utils
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class JenkinsAgentAttribute : Attribute
    {
        public JenkinsAgentAttribute(JenkinsAgentType agentType, JenkinsAgentPlatform agentPlatform)
        {
            AgentType = agentType;
            AgentPlatform = agentPlatform;
        }

        public JenkinsAgentType AgentType { get; }

        public JenkinsAgentPlatform AgentPlatform { get; }

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
    }
}