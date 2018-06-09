namespace ClassLibrary1.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ConvertPrimaryKeyToGuid : DbMigration
    {
        public override void Up()
        {
            DropPrimaryKey("dbo.Students");
            RenameColumn("dbo.Students", "Id", "TempId");
            AddColumn("dbo.Students", "Id", c => c.Guid(nullable: false));
            Sql(@"
                UPDATE s
                SET s.Id = CAST(
                    CONVERT(
                        BINARY(16),
                        REVERSE(
                            CONVERT(BINARY(16), s.TempId)
                        )
                    ) as uniqueidentifier
                )
                FROM dbo.Students s
            ");
            DropColumn("dbo.Students", "TempId");
            AddPrimaryKey("dbo.Students", "Id");
        }
        
        public override void Down()
        {
            DropPrimaryKey("dbo.Students");
            RenameColumn("dbo.Students", "Id", "TempId");
            AddColumn("dbo.Students", "Id", c => c.Int(nullable: false));
            Sql(@"
                CREATE TABLE OverflowIdStudents(
                    Name nvarchar(24)
                );

                INSERT INTO OverflowIdStudents
                    (Name)
                    SELECT s.Name
                    FROM dbo.Students s
                    WHERE CONVERT(nvarchar(36), s.Id) NOT LIKE '%-0000-0000-0000-000000000000';

                DELETE s
                FROM dbo.Students s
                WHERE CONVERT(nvarchar(36), s.Id) NOT LIKE '%-0000-0000-0000-000000000000'
            ");

            Sql(@"
                UPDATE s
                SET s.Id = CAST(
                    CONVERT(
                        BINARY(16),
                        REVERSE(CONVERT(BINARY(16), s.TempId))
                    ) as int
                )
                FROM dbo.Students s
            ");

            DropColumn("dbo.Students", "TempId");
            AlterColumn("dbo.Students", "Id", c => c.Int(nullable: false, identity: true));
            AddPrimaryKey("dbo.Students", "Id");

            Sql(@"
                INSERT INTO dbo.Students (Name)
                    SELECT s.Name FROM OverflowIdStudents s;

                DROP TABLE OverflowIdStudents;
            ");
        }
    }
}
