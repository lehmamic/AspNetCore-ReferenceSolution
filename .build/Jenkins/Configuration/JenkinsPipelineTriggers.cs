using Nuke.Common.CI;
using Nuke.Common.Utilities;

namespace Jenkins.Configuration
{
    public class JenkinsPipelineTriggers : ConfigurationEntity
    {
        public string CronTrigger { get; set; }

        public string PollTrigger { get; set; }

        public override void Write(CustomFileWriter writer)
        {
            if (!string.IsNullOrWhiteSpace(CronTrigger) || !string.IsNullOrWhiteSpace(PollTrigger))
            {
                using (writer.WriteBlock("triggers"))
                {
                    if (!string.IsNullOrWhiteSpace(CronTrigger))
                    {
                        writer.WriteLine($"cron('{CronTrigger}')");
                    }

                    if (!string.IsNullOrWhiteSpace(PollTrigger))
                    {
                        writer.WriteLine($"pollSCM('{PollTrigger}')");
                    }
                }
            }
        }
    }
}