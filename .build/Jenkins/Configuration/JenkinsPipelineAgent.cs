using Nuke.Common.CI;
using Nuke.Common.Utilities;

namespace Jenkins.Configuration
{
    public class JenkinsPipelineAgent : ConfigurationEntity
    {
        public override void Write(CustomFileWriter writer)
        {
            writer.WriteLine("agent any");
        }
    }
}