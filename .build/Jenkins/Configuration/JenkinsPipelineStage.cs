using Nuke.Common.CI;
using Nuke.Common.Utilities;
using Nuke.Common.Utilities.Collections;

namespace Jenkins.Configuration
{
    public class JenkinsPipelineStage : ConfigurationEntity
    {
        public string Name { get; set; }
        
        public bool IsCheckoutScm  { get; set; }

        public string InvokedTarget { get; set; }

        public JenkinsPipelineAgent Agent { get; set; }

        public JenkinsPipelineStash[] Stashes { get; set; }
        
        public JenkinsPipelineUnstash[] Unstashes { get; set; }

        public override void Write(CustomFileWriter writer)
        {
            using (writer.WriteBlock($"stage('{Name}')"))
            {
                Agent.Write(writer);

                using (writer.WriteBlock($"steps"))
                {
                    Unstashes?.ForEach(s => s.Write(writer));

                    if (IsCheckoutScm)
                    {
                        writer.WriteLine($"checkout scm");
                    }

                    if (!string.IsNullOrWhiteSpace(InvokedTarget))
                    {
                        writer.WriteLine($"sh 'sh ./build.sh --target {InvokedTarget}' --skip");
                    }

                    Stashes?.ForEach(s => s.Write(writer));
                }
            }
        }
    }
}