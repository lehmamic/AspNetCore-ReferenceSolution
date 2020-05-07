using DbUp;
using DbUp.Engine;
using DbUp.Helpers;
using Demo.Tools.DatabaseMigrator.Scripts;
using Demo.Tools.DatabaseMigrator.Utils;
using JetBrains.Annotations;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Data.SqlClient;
using System.Threading;
using System.Threading.Tasks;

namespace Demo.Tools.DatabaseMigrator.Commands
{
    [UsedImplicitly]
    public class DropDatabaseCommandHandler : IRequestHandler<DropDatabaseCommand, int>
    {
        private readonly ILogger<DropDatabaseCommandHandler> _logger;

        public DropDatabaseCommandHandler([NotNull] ILogger<DropDatabaseCommandHandler> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public Task<int> Handle(DropDatabaseCommand request, CancellationToken cancellationToken)
        {
            _logger.LogDebug("Running with Options={@DbUpOptions}", request);

            var connectionStringBuilder = new SqlConnectionStringBuilder(request.ConnectionString);
            string databaseName = connectionStringBuilder.InitialCatalog;

            connectionStringBuilder.InitialCatalog = "master";

            DatabaseUpgradeResult result = DeployChanges.To.SqlDatabase(connectionStringBuilder.ToString())
                .WithScript("DROP_DATABASE", new DropDatabaseScript(databaseName))
                .LogToLogger(_logger)
                .JournalTo(new NullJournal())
                .Build()
                .PerformUpgrade();

            return Task.FromResult(result.Successful ? 0 : 1);
        }
    }
}