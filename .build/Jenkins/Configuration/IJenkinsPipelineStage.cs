using Nuke.Common.Utilities;

namespace Jenkins.Configuration
{
    public interface IJenkinsPipelineStage
    {
        string Name { get; set; }
        void Write(CustomFileWriter writer);
    }
}