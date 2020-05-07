using DbUp.Engine.Output;
using JetBrains.Annotations;
using Microsoft.Extensions.Logging;
using System;

namespace Demo.Tools.DatabaseMigrator.Utils
{
    public class DbUpLogger<T> : IUpgradeLog
    {
        private readonly ILogger<T> _logger;

        public DbUpLogger([NotNull] ILogger<T> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public void WriteInformation(string format, params object[] args)
        {
            _logger.LogInformation(format, args);
        }

        public void WriteError(string format, params object[] args)
        {
            _logger.LogError(format, args);
        }

        public void WriteWarning(string format, params object[] args)
        {
            _logger.LogWarning(format, args);
        }
    }
}