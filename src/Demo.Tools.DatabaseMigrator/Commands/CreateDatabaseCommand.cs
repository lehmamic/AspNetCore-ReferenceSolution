using CommandLine;
using JetBrains.Annotations;

namespace Demo.Tools.DatabaseMigrator.Commands
{
    [UsedImplicitly]
    [Verb("create", HelpText = "Creates the specified DB.")]
    public class CreateDatabaseCommand : CommandBase
    {
        [UsedImplicitly]
        [Option('c', "connection-string", Required = true, HelpText = "Connection string for the DB to create.")]
        public string ConnectionString { get; set; } = null!;
    }
}