using DbUp.Engine;
using JetBrains.Annotations;
using System;
using System.Data;

namespace Demo.Tools.DatabaseMigrator.Scripts
{
    public class DropDatabaseScript : IScript
    {
        private readonly string _databaseName;

        public DropDatabaseScript([NotNull] string databaseName)
        {
            _databaseName = databaseName ?? throw new ArgumentNullException(nameof(databaseName));
        }

        public string ProvideScript(Func<IDbCommand> dbCommandFactory)
        {
            IDbCommand command = dbCommandFactory.Invoke();
            command.CommandType = CommandType.Text;
            command.CommandText = @"
            IF DB_ID(N'%s') IS NOT NULL BEGIN
            ALTER DATABASE [%s] SET SINGLE_USER WITH ROLLBACK IMMEDIATE
                DROP DATABASE IF EXISTS [%s]
            END".Replace(@"%s", _databaseName);

            command.ExecuteNonQuery();

            return string.Empty;
        }
    }
}