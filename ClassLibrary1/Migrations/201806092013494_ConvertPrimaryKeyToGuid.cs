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
        }
    }
}
