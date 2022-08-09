using System;
using System.Threading.Tasks;
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Configurations;
using DotNet.Testcontainers.Containers;
using RabbitMQ.Client;
using Xunit;

namespace IntegrationTest;

public class RabbitMqTest : IAsyncLifetime
{
    private readonly RabbitMqTestcontainer _container;

    public RabbitMqTest()
    {
        var builder = new TestcontainersBuilder<RabbitMqTestcontainer>()
            .WithMessageBroker(new RabbitMqTestcontainerConfiguration
            {
                Username = "rabbitmq",
                Password = "rabbitmq",
                Port = 5673
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
        var factory = new ConnectionFactory { Uri = new Uri(_container.ConnectionString) };

        using (var connection = factory.CreateConnection())
        {
            Assert.True(connection.IsOpen);
        }
    }
}