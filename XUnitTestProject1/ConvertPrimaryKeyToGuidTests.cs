using System;
using System.Data.Entity.Migrations;
using Xunit;

namespace XUnitTestProject1
{
    public class ConvertPrimaryKeyToGuidTests
    { 
        [Fact]
        public void Up_ShouldConvertIdToGuid()
        {
            var expected = "ABC";
            Migrate.From<ClassLibrary1.Migrations.Initial>()
                .Setup(
                    conn => conn.ExecuteScalar<int>(cmd =>
                    {
                        cmd.CommandText = @"
                            INSERT INTO dbo.Students (Name) VALUES(@Name);
                            SELECT CAST(@@IDENTITY AS int);
                        ";
                        cmd.Parameters.AddWithValue("@Name", expected);
                    })
                )
                .To<ClassLibrary1.Migrations.ConvertPrimaryKeyToGuid>()
                .Assert((id, conn) =>
                {
                    var convertedId = new Guid($"{id:00000000}-0000-0000-0000-000000000000");

                    var actual = conn.ExecuteScalar<string>(cmd =>
                    {
                        cmd.CommandText = @"
                            SELECT s.Name FROM dbo.Students s WHERE s.Id = @Id
                        ";
                        cmd.Parameters.AddWithValue("@Id", convertedId);
                    });

                    Assert.Equal(expected, actual);
                });
        }
    }
}
