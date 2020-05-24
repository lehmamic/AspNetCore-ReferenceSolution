using Nuke.Common;
using Nuke.Common.Execution;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Jenkins.Utils
{
    public static class ExecutableTargetExtensions
    {
        public static ITargetDefinition GetDefinition(this ExecutableTarget executableTarget)
        {
            PropertyInfo property = typeof(ExecutableTarget).GetProperty(
                    "Definition", 
                    BindingFlags.NonPublic | BindingFlags.Instance);

            if (property == null)
            {
                throw new InvalidOperationException($"The property Definition does not exist on type {nameof(ExecutableTarget)}");
            }

            return property?.GetValue(executableTarget) as ITargetDefinition;
        }

        public static IEnumerable<string> GetArtifactProducts(this ExecutableTarget executableTarget)
        {
            return ArtifactExtensionsAccessor.ArtifactProducts[executableTarget.GetDefinition()];
        }

        public static ExecutableTarget ResolveExecutionDependency(
            this ExecutableTarget executableTarget,
            Target target)
        {
            return executableTarget.ExecutionDependencies.Single(x => x.Factory == target);
        }
    }
}