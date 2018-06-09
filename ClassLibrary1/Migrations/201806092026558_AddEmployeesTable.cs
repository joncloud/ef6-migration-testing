namespace ClassLibrary1.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddEmployeesTable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Employees",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        PersonId = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.People", t => t.PersonId, cascadeDelete: true)
                .Index(t => t.PersonId);
            
            CreateTable(
                "dbo.People",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Name = c.String(maxLength: 24),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("dbo.Students", "PersonId", c => c.Guid(nullable: false));
            CreateIndex("dbo.Students", "PersonId");

            Sql(@"
                UPDATE s
                SET s.PersonId = NEWID()
                FROM dbo.Students s
            ");

            Sql(@"
                INSERT INTO dbo.People
                    (Id, Name)
                    SELECT s.PersonId, s.Name
                    FROM dbo.Students s
            ");

            AddForeignKey("dbo.Students", "PersonId", "dbo.People", "Id", cascadeDelete: true);
            DropColumn("dbo.Students", "Name");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Students", "Name", c => c.String(maxLength: 24));

            Sql(@"
                UPDATE s
                SET s.Name = p.Name
                FROM dbo.Students s
                INNER JOIN dbo.People p
                ON p.Id = s.PersonId
            ");

            DropForeignKey("dbo.Students", "PersonId", "dbo.People");
            DropForeignKey("dbo.Employees", "PersonId", "dbo.People");
            DropIndex("dbo.Students", new[] { "PersonId" });
            DropIndex("dbo.Employees", new[] { "PersonId" });
            DropColumn("dbo.Students", "PersonId");
            DropTable("dbo.People");
            DropTable("dbo.Employees");
        }
    }
}
