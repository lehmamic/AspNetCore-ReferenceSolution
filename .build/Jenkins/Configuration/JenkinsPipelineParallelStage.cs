using Nuke.Common.CI;
using Nuke.Common.Utilities;
using Nuke.Common.Utilities.Collections;

namespace Jenkins.Configuration
{
    public class JenkinsPipelineParallelStage : ConfigurationEntity, IJenkinsPipelineStage
    {
        public string Name { get; set; }

        public IJenkinsPipelineStage[] Stages { get; set; }

        public override void Write(CustomFileWriter writer)
        {
            using (writer.WriteBlock($"stage('{Name}')"))
            {
                writer.WriteLine("failFast true");
                using (writer.WriteBlock($"parallel"))
                {
                    Stages.ForEach(x => x.Write(writer));
                }
            }
        }
    }
}