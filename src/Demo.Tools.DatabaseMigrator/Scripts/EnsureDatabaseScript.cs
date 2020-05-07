using System;
using System.Data;
using DbUp.Engine;
using JetBrains.Annotations;

namespace Demo.Tools.DatabaseMigrator.Scripts
{
    public class EnsureDatabaseScript : IScript
    {
        private readonly string _databaseName;

        public EnsureDatabaseScript([NotNull] string databaseName)
        {
            _databaseName = databaseName ?? throw new ArgumentNullException(nameof(databaseName));
        }

        public string ProvideScript(Func<IDbCommand> dbCommandFactory)
        {
            IDbCommand command = dbCommandFactory.Invoke();
            command.CommandType = CommandType.Text;
            command.CommandText = $"IF DB_ID (N'{_databaseName}') IS NULL CREATE DATABASE [{_databaseName}]";

            command.ExecuteNonQuery();

            return string.Empty;
        }
    }
}