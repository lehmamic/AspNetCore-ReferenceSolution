using Nuke.Common;
using Nuke.Common.CI;
using Nuke.Common.Tooling;
using System;
using System.Reflection;

namespace Jenkins.Utils
{
    public static class ArtifactExtensionsAccessor
    {
        public static LookupTable<ITargetDefinition, string> ArtifactProducts
        {
            get
            {
                FieldInfo field = typeof(ArtifactExtensions).GetField(
                    "ArtifactProducts",
                    BindingFlags.Static | BindingFlags.NonPublic);

                if (field == null)
                {
                    throw new InvalidOperationException($"The field ArtifactProducts does not exist on type {nameof(ArtifactExtensions)}");
                }

                return (LookupTable<ITargetDefinition, string>)field.GetValue(null);
            }
        }

        public static LookupTable<ITargetDefinition, (Target, string[])> ArtifactDependencies
        {
            get
            {
                FieldInfo field = typeof(ArtifactExtensions).GetField(
                    "ArtifactDependencies",
                    BindingFlags.Static | BindingFlags.NonPublic);

                if (field == null)
                {
                    throw new InvalidOperationException($"The field ArtifactDependencies does not exist on type {nameof(ArtifactExtensions)}");
                }

                return (LookupTable<ITargetDefinition, (Target, string[])>)field.GetValue(null);
            }
        }
    }
}