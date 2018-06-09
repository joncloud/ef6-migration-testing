using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace XUnitTestProject1
{
    static class SqlConnectionExtensions
    {
        public static int ExecuteNonQuery(this SqlConnection connection, Action<SqlCommand> fn)
        {
            var command = connection.CreateCommand();
            fn(command);
            return command.ExecuteNonQuery();
        }

        public static IEnumerable<T> ExecuteReader<T>(this SqlConnection connection, Action<SqlCommand> fn, Func<SqlDataReader, T> read)
        {
            var command = connection.CreateCommand();
            fn(command);

            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    yield return read(reader);
                }
            }
        }

        public static T ExecuteScalar<T>(this SqlConnection connection, Action<SqlCommand> fn)
        {
            var command = connection.CreateCommand();
            fn(command);
            return (T)command.ExecuteScalar();
        }
    }
}
