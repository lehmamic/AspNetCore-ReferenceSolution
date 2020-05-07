using CommandLine;
using MediatR;

namespace Demo.Tools.DatabaseMigrator.Commands
{
    public class CommandBase : IRequest<int>
    {
        [Option("verbose", Default = false, Required = false, HelpText = "Enables verbose output.")]
        public bool Verbose { get; set; }
    }
}