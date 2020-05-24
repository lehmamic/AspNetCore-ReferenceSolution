using Nuke.Common.CI;
using Nuke.Common.Utilities;

namespace Jenkins.Configuration
{
    public class JenkinsPipelineUnstash : ConfigurationEntity
    {
        public string Name { get; set; }

        public override void Write(CustomFileWriter writer)
        {
            writer.WriteLine($"unstash '{Name}'");
        }
    }
}