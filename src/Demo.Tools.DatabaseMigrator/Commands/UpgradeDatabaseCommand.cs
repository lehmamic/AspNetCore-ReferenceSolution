using CommandLine;
using JetBrains.Annotations;

namespace Demo.Tools.DatabaseMigrator.Commands
{
    [UsedImplicitly]
    [Verb("up", HelpText = "Upgrades specified DB with migration scripts.")]
    public class UpgradeDatabaseCommand : CommandBase
    {
        [UsedImplicitly]
        [Option('c', "connection-string", Required = true, HelpText = "Connection string for the DB to upgrade.")]
        public string ConnectionString { get; set; } = null!;

        [UsedImplicitly]
        [Option('s', "scripts-directory", Required = true, HelpText = "Folder containing the migration scripts.")]
        public string ScriptsDirectory { get; set; } = null!;

        [UsedImplicitly]
        [Option('e', "environment", HelpText = "The environment in which the database is used. (Default is local")]
        public string Environment { get; set; } = "local";
    }
}