using System;
using System.Data.Entity.Migrations;
using System.Data.SqlClient;

namespace XUnitTestProject1
{
    public struct SourceMigration
    {
        readonly string _migrationName;
        public SourceMigration(string migrationName)
        {
            _migrationName = migrationName;
        }

        public SetupMigration<T> Setup<T>(Func<SqlConnection, T> fn) =>
            new SetupMigration<T>(this, fn);

        public void Apply<T>(DbMigratorHarness<T> harness)
            where T : DbMigrationsConfiguration, new()
        {
            harness.TargetMigration(_migrationName);
        }
    }
}
