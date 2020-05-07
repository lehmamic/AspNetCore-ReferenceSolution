using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DbUp;
using DbUp.Engine;
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

            var variables = new Dictionary<string, string> { { "environment", request.Environment } };

            DatabaseUpgradeResult result = DeployChanges.To.SqlDatabase(request.ConnectionString)
                .WithScriptsFromFileSystem(request.ScriptsDirectory)
                .WithTransaction()
                .LogToLogger(_logger)
                .WithVariables(variables)
                .Build()
                .PerformUpgrade();

            return Task.FromResult(result.Successful ? 0 : 1);
        }
    }
}