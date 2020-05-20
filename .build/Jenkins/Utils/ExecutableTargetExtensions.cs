using Nuke.Common;
using Nuke.Common.Execution;
using System;
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
            
            Console.WriteLine(property.Name);

            return property?.GetValue(executableTarget) as ITargetDefinition;
        }
    }
}