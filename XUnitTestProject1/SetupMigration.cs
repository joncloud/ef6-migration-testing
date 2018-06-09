using System;
using System.Data.Entity.Migrations;
using System.Data.SqlClient;

namespace XUnitTestProject1
{
    public struct SetupMigration<T>
    {
        readonly Func<SqlConnection, T> _fn;
        readonly SourceMigration _sourceMigration;
        public SetupMigration(SourceMigration sourceMigration, Func<SqlConnection, T> fn)
        {
            _sourceMigration = sourceMigration;
            _fn = fn;
        }

        public TargetMigration<T> To<TMigration>() where TMigration : DbMigration =>
            To(typeof(TMigration).Name);

        public TargetMigration<T> To(string migrationName) =>
            new TargetMigration<T>(_sourceMigration, _fn, migrationName);
    }
}
