using Nuke.Common.Tools.GitVersion;

namespace Utils
{
    public static class GitVersionExtensions
    {
        public static string GetDockerTag(this GitVersion gitVersion)
        {
            return gitVersion.FullSemVer.Replace("+", ".");
        }
    }
}