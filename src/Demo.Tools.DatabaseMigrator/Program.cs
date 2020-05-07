using System.Reflection;
using System.Threading.Tasks;
using CommandLine;
using Demo.Tools.DatabaseMigrator.Commands;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Core;
using Serilog.Events;
using Serilog.Extensions.Logging;

namespace Demo.Tools.DatabaseMigrator
{
    public class Program
    {
        public static async Task<int> Main(string[] args)
        {
            var levelSwitch = new LoggingLevelSwitch()
            {
                MinimumLevel = LogEventLevel.Information,
            };

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.ControlledBy(levelSwitch)
                .Enrich.FromLogContext()
                .WriteTo.Console(outputTemplate: "{Timestamp:HH:mm:ss} [{Level:u3}] {SourceContext} {Message:lj}{NewLine}{Exception}")
                .CreateLogger();

            ServiceProvider serviceProvider = new ServiceCollection()
                .AddLogging()
                .AddSingleton<ILoggerFactory>(services => new SerilogLoggerFactory(Log.Logger, false))
                .AddMediatR(Assembly.GetExecutingAssembly())
                .BuildServiceProvider();

            return await Parser.Default.ParseArguments<UpgradeDatabaseCommand, CreateDatabaseCommand, DropDatabaseCommand>(args)
                .MapResult(
                async (CommandBase command) =>
                {
                    if (command.Verbose)
                    {
                        levelSwitch.MinimumLevel = LogEventLevel.Verbose;
                    }

                    return await serviceProvider.GetRequiredService<IMediator>().Send(command);
                },
                _ => Task.FromResult(1));
        }
    }
}