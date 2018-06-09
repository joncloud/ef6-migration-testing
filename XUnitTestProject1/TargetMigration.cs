using System;
using System.Data.SqlClient;
using SchoolDbConfiguration = ClassLibrary1.Migrations.Configuration;

namespace XUnitTestProject1
{
    public struct TargetMigration<T>
    {
        readonly string _migrationName;
        readonly Func<SqlConnection, T> _fn;
        readonly SourceMigration _sourceMigration;
        public TargetMigration(SourceMigration sourceMigration, Func<SqlConnection, T> fn, string migrationName)
        {
            _sourceMigration = sourceMigration;
            _fn = fn;
            _migrationName = migrationName;
        }

        public void Assert(Action<T, SqlConnection> fn)
        {
            using (var harness = new DbMigratorHarness<SchoolDbConfiguration>())
            {
                _sourceMigration.Apply(harness);

                T arrangement = harness.UseConnection(_fn);

                harness.TargetMigration(_migrationName);

                harness.UseConnection(conn => fn(arrangement, conn));
            }
        }
    }
}
