using System;
using System.Linq;
using Xunit;

namespace XUnitTestProject1
{
    public class AddEmployeesTableTests
    {
        [Fact]
        public void Up_ShouldMoveStudentNamesToPeople()
        {
            var id = Guid.NewGuid();
            var name = "ABC";
            Migrate.From<ClassLibrary1.Migrations.ConvertPrimaryKeyToGuid>()
                .Setup(
                    conn => conn.ExecuteNonQuery(cmd =>
                    {
                        cmd.CommandText = @"
                            INSERT INTO dbo.Students (Id, Name) VALUES(@Id, @Name);
                        ";
                        cmd.Parameters.AddWithValue("@Id", id);
                        cmd.Parameters.AddWithValue("@Name", name);
                    })
                )
                .To<ClassLibrary1.Migrations.AddEmployeesTable>()
                .Assert((_, conn) =>
                {
                    var result = conn.ExecuteReader(
                        cmd =>
                        {
                            cmd.CommandText = @"
                                SELECT p.Id, p.Name
                                FROM dbo.Students s
                                INNER JOIN dbo.People p
                                ON p.Id = s.PersonId
                                WHERE s.Id = @Id
                            ";

                            cmd.Parameters.AddWithValue("@Id", id);
                        },
                        reader => (reader.GetGuid(0), reader.GetString(1))
                    )
                    .First();

                    Assert.NotEqual(id, result.Item1);
                    Assert.Equal(name, result.Item2);
                });
        }
    }
}
