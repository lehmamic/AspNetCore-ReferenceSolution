using DbUp.Builder;
using JetBrains.Annotations;
using Microsoft.Extensions.Logging;
using System;

namespace Demo.Tools.DatabaseMigrator.Utils
{
    public static class UpgradeEngineBuilderExtensions
    {
        public static UpgradeEngineBuilder LogToLogger<T>([NotNull] this UpgradeEngineBuilder builder, [NotNull] ILogger<T> logger)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            if (logger == null)
            {
                throw new ArgumentNullException(nameof(logger));
            }

            builder.LogTo(new DbUpLogger<T>(logger));

            return builder;
        }
    }
}