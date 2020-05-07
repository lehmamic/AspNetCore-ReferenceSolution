using CommandLine;
using JetBrains.Annotations;

namespace Demo.Tools.DatabaseMigrator.Commands
{
    [UsedImplicitly]
    [Verb("drop", HelpText = "Drops the specified DB.")]
    public class DropDatabaseCommand : CommandBase
    {
        [UsedImplicitly]
        [Option('c', "connection-string", Required = true, HelpText = "Connection string for the DB to drop.")]
        public string ConnectionString { get; set; } = null!;
    }
}