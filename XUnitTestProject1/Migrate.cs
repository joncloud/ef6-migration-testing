using System.Data.Entity.Migrations;

namespace XUnitTestProject1
{
    public static class Migrate
    {
        public static SourceMigration From(string migrationName) =>
            new SourceMigration(migrationName);

        public static SourceMigration From<T>() where T : DbMigration =>
            From(typeof(T).Name);
    }
}
