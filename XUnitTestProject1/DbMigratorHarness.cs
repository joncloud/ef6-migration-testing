using System;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Migrations;
using System.Data.SqlClient;

namespace XUnitTestProject1
{
    public class DbMigratorHarness<T> : IDisposable
        where T : DbMigrationsConfiguration, new()
    {
        readonly string _connectionString;
        readonly DbMigrator _migrator;
        public DbMigratorHarness()
        {
            _connectionString = CreateNewSqlConnection();
            CreateDatabase();

            var dbConnectionInfo = new DbConnectionInfo(_connectionString, "System.Data.SqlClient");
            var configuration = new T
            {
                TargetDatabase = dbConnectionInfo
            };
            _migrator = new DbMigrator(configuration);
        }

        static string CreateNewSqlConnection()
        {
            var baseConnectionString = "Data Source=.;Initial Catalog=SchoolDbContext;Integrated Security=true;";
            var builder = new SqlConnectionStringBuilder(baseConnectionString);
            builder.InitialCatalog += Guid.NewGuid().ToString();
            return builder.ConnectionString;
        }

        string GetDatabaseName()
        {
            var builder = new SqlConnectionStringBuilder(_connectionString);
            return builder.InitialCatalog;
        }

        string GetMasterConnectionString()
        {
            var builder = new SqlConnectionStringBuilder(_connectionString);
            builder.InitialCatalog = "master";
            return builder.ConnectionString;
        }

        void CreateDatabase()
        {
            var masterConnectionString = GetMasterConnectionString();
            using (var connection = new SqlConnection(masterConnectionString))
            {
                connection.Open();

                using (var command = connection.CreateCommand())
                {
                    var databaseName = GetDatabaseName();
                    command.CommandText = $@"
                        CREATE DATABASE [{databaseName}]
                    ";
                    command.ExecuteNonQuery();
                }
            }
        }

        void DropDatabaseIfExists()
        {
            var masterConnectionString = GetMasterConnectionString();
            using (var connection = new SqlConnection(masterConnectionString))
            {
                connection.Open();

                using (var command = connection.CreateCommand())
                {
                    var databaseName = GetDatabaseName();
                    command.CommandText = $@"
                        IF EXISTS(SELECT 1 FROM sys.databases WHERE name = '{databaseName}')
                        BEGIN
                            ALTER DATABASE [{databaseName}] SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
                            DROP DATABASE [{databaseName}];
                        END
                    ";
                    command.ExecuteNonQuery();
                }
            }
        }

        public void UseConnection(Action<SqlConnection> fn)
        {
            UseConnection(conn =>
            {
                fn(conn);
                return 0;
            });
        }

        public TResult UseConnection<TResult>(Func<SqlConnection, TResult> fn)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                return fn(connection);
            }
        }

        public DbMigratorHarness<T> TargetMigration(string migrationName)
        {
            _migrator.Update(migrationName);
            return this;
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                }

                DropDatabaseIfExists();

                disposedValue = true;
            }
        }
        
        ~DbMigratorHarness()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(false);
        }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}
