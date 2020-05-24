using Jenkins.Utils;
using Nuke.Common;
using Nuke.Common.Execution;
using Nuke.Common.Tooling;
using System.Collections.Generic;

namespace Jenkins
{
    public static class AgentExtensions
    {
        internal static readonly LookupTable<ITargetDefinition, string> Agents =
            new LookupTable<ITargetDefinition, string>();
        
        public static ITargetDefinition RunsOnAgent(this ITargetDefinition targetDefinition, params string[] agents)
        {
            Agents.AddRange(targetDefinition, agents);
            return targetDefinition;
        }
        
        public static IEnumerable<string> GetAgents(this ExecutableTarget executableTarget)
        {
            return Agents[executableTarget.GetDefinition()];
        }
    }
}