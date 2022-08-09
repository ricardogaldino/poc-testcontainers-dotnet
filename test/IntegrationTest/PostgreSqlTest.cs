using System.Data;
using System.Threading.Tasks;
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Configurations;
using DotNet.Testcontainers.Containers;
using Npgsql;
using Xunit;

namespace IntegrationTest;

public class PostgreSqlTest : IAsyncLifetime
{
    private readonly PostgreSqlTestcontainer _container;

    public PostgreSqlTest()
    {
        var builder = new TestcontainersBuilder<PostgreSqlTestcontainer>()
            .WithDatabase(new PostgreSqlTestcontainerConfiguration
            {
                Database = "db",
                Username = "postgres",
                Password = "postgres",
                Port = 5433
            });

        _container = builder.Build();
    }

    public async Task InitializeAsync()
    {
        await _container.StartAsync();
    }

    public async Task DisposeAsync()
    {
        await _container.StopAsync();
    }

    [Fact]
    public void ShouldOpenConnectionSuccessfully()
    {
        using (var connection = new NpgsqlConnection(_container.ConnectionString))
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