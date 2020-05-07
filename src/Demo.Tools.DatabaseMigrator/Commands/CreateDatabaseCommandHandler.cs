using System;
using System.Data.SqlClient;
using System.Threading;
using System.Threading.Tasks;
using DbUp;
using DbUp.Engine;
using DbUp.Helpers;
using Demo.Tools.DatabaseMigrator.Scripts;
using Demo.Tools.DatabaseMigrator.Utils;
using JetBrains.Annotations;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Demo.Tools.DatabaseMigrator.Commands
{
    [UsedImplicitly]
    public class CreateDatabaseCommandHandler : IRequestHandler<CreateDatabaseCommand, int>
    {
        private readonly ILogger<CreateDatabaseCommandHandler> _logger;

        public CreateDatabaseCommandHandler([NotNull] ILogger<CreateDatabaseCommandHandler> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public Task<int> Handle(CreateDatabaseCommand request, CancellationToken cancellationToken)
        {
            _logger.LogDebug("Running with Options={@DbUpOptions}", request);

            var connectionStringBuilder = new SqlConnectionStringBuilder(request.ConnectionString);
            string databaseName = connectionStringBuilder.InitialCatalog;

            connectionStringBuilder.InitialCatalog = "master";

            DatabaseUpgradeResult result = DeployChanges.To.SqlDatabase(connectionStringBuilder.ToString())
                .WithScript("ENSURE_DATABASE", new EnsureDatabaseScript(databaseName))
                .LogToLogger(_logger)
                .JournalTo(new NullJournal())
                .Build()
                .PerformUpgrade();

            return Task.FromResult(result.Successful ? 0 : 1);
        }
    }
}