using Nuke.Common.CI;
using Nuke.Common.Utilities;
using Nuke.Common.Utilities.Collections;

namespace Jenkins.Configuration
{
    public class JenkinsConfiguration : ConfigurationEntity
    {
        public JenkinsPipelineAgent Agent { get; set; }

        public JenkinsPipelineTriggers Triggers { get; set; }

        public IJenkinsPipelineStage[] Stages { get; set; }

        public override void Write(CustomFileWriter writer)
        {
            using (writer.WriteBlock("pipeline"))
            {
                Agent.Write(writer);
                Triggers.Write(writer);

                using (writer.WriteBlock("stages"))
                {
                    Stages.ForEach(x => x.Write(writer));
                }
            }
        }
    }
}