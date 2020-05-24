using Nuke.Common.CI;
using Nuke.Common.Utilities;
using System.Collections.Generic;
using System.Linq;

namespace Jenkins.Configuration
{
    public class JenkinsPipelineStash : ConfigurationEntity
    {
        public string Name { get; set; }

        public string Includes { get; set; }

        public string Excludes { get; set; }

        public override void Write(CustomFileWriter writer)
        {
            IEnumerable<string> arguments = new[] {$"name: '{Name}'"};

            if (!string.IsNullOrWhiteSpace(Includes))
            {
                arguments = arguments.Append($"includes: '{Includes}'");
            }

            if (!string.IsNullOrWhiteSpace(Excludes))
            {
                arguments = arguments.Append($"excludes: '{Excludes}'");
            }

            writer.WriteLine($"stash {string.Join(", ", arguments)}");
        }
    }
}