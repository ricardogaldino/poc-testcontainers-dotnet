using System;
using System.Threading.Tasks;
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Configurations;
using DotNet.Testcontainers.Containers;
using RabbitMQ.Client;
using Xunit;

namespace IntegrationTest;

public class RabbitMqTest
{
    [Fact]
    public async Task RabbitMqContainer()
    {
        var builder = new TestcontainersBuilder<RabbitMqTestcontainer>()
            .WithMessageBroker(new RabbitMqTestcontainerConfiguration
            {
                Username = "rabbitmq",
                Password = "rabbitmq",
                Port = 5673
            });

        await using (var container = builder.Build())
        {
            await container.StartAsync();

            var factory = new ConnectionFactory {Uri = new Uri(container.ConnectionString)};

            using (var connection = factory.CreateConnection())
            {
                Assert.True(connection.IsOpen);
            }
        }
    }
}