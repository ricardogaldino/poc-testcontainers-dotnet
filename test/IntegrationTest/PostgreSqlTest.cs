using System.Data;
using System.Threading.Tasks;
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Configurations;
using DotNet.Testcontainers.Containers;
using Npgsql;
using Xunit;

namespace IntegrationTest;

public class PostgreSqlTest

{
    [Fact]
    public async Task PostgreSqlContainer()
    {
        var builder = new TestcontainersBuilder<PostgreSqlTestcontainer>()
            .WithDatabase(new PostgreSqlTestcontainerConfiguration
            {
                Database = "db",
                Username = "postgres",
                Password = "postgres",
                Port = 5433
            });

        await using (var container = builder.Build())
        {
            await container.StartAsync();

            using (var connection = new NpgsqlConnection(container.ConnectionString))
            {
                using (var command = new NpgsqlCommand())
                {
                    connection.Open();
                    command.Connection = connection;
                    command.CommandText = "SELECT 1";
                    command.ExecuteReader();

                    Assert.True(connection.State == ConnectionState.Open);
                }
            }
        }
    }
}