using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading;
using System.Threading.Tasks;
using DbUp;
using DbUp.Builder;
using DbUp.Engine;
using DbUp.Helpers;
using Demo.Tools.DatabaseMigrator.Utils;
using JetBrains.Annotations;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Demo.Tools.DatabaseMigrator.Commands
{
    [UsedImplicitly]
    public class UpgradeDatabaseCommandHandler : IRequestHandler<UpgradeDatabaseCommand, int>
    {
        private readonly ILogger<UpgradeDatabaseCommandHandler> _logger;

        public UpgradeDatabaseCommandHandler([NotNull] ILogger<UpgradeDatabaseCommandHandler> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public Task<int> Handle(UpgradeDatabaseCommand request, CancellationToken cancellationToken)
        {
            _logger.LogDebug("Running with Options={@DbUpOptions}", request);

            var connectionStringBuilder = new SqlConnectionStringBuilder(request.ConnectionString);

            var variables = new Dictionary<string, string>
            {
                { "environment", request.Environment },
                { "login_name", connectionStringBuilder.UserID },
                { "database_name", connectionStringBuilder.InitialCatalog },
            };

            UpgradeEngineBuilder builder = DeployChanges.To.SqlDatabase(request.ConnectionString)
                .WithScriptsFromFileSystem(request.ScriptsDirectory)
                .WithTransaction()
                .LogToLogger(_logger)
                .WithVariables(variables);

            if (!request.EnableJournaling)
            {
                builder = builder.JournalTo(new NullJournal());
            }

            DatabaseUpgradeResult result = builder
                .Build()
                .PerformUpgrade();

            return Task.FromResult(result.Successful ? 0 : 1);
        }
    }
}