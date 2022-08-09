using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Amazon.SimpleEmail;
using Amazon.SimpleEmail.Model;
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;
using Xunit;

namespace IntegrationTest;

public class AwsSesTest : IAsyncLifetime
{
    private readonly TestcontainersContainer _container;

    public AwsSesTest()
    {
        var builder = new TestcontainersBuilder<TestcontainersContainer>()
            .WithImage("localstack/localstack")
            .WithCleanUp(true)
            .WithEnvironment("DEFAULT_REGION", "us-west-2")
            .WithEnvironment("SERVICES", "ses")
            .WithEnvironment("AWS_ACCESS_KEY_ID", "6b38f1ec442547fa93b8e2f64559bd3f")
            .WithEnvironment("AWS_SECRET_ACCESS_KEY", "17e6025cabc848c6b7757c7bf08bf4b6")
            .WithEnvironment("DOCKER_HOST", "unix:///var/run/docker.sock")
            .WithEnvironment("DEBUG", "1")
            .WithPortBinding(4567, 4566);

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
    public async Task ShouldSendEmailSuccessfully()
    {
        using (var client = new AmazonSimpleEmailServiceClient(
                   "6b38f1ec442547fa93b8e2f64559bd3f",
                   "17e6025cabc848c6b7757c7bf08bf4b6",
                   new AmazonSimpleEmailServiceConfig { ServiceURL = "http://localhost:4567" }
               ))
        {
            var request = new SendEmailRequest
            {
                Source = "sender@email.com",
                Destination = new Destination
                {
                    ToAddresses = new List<string> { "receiver@email.com" }
                },
                Message = new Message
                {
                    Subject = new Content
                    {
                        Charset = "UTF-8",
                        Data = "Mussum Ipsum"
                    },
                    Body = new Body
                    {
                        Html = new Content
                        {
                            Charset = "UTF-8",
                            Data = @"Cacilds vidis litro abertis. 
                         Suco de cevadiss, é um leite divinis, qui tem lupuliz, matis, aguis e fermentis.
                         Quem num gosta di mé, boa gentis num é.
                         Quem num gosta di mim que vai caçá sua turmis!
                         Suco de cevadiss deixa as pessoas mais interessantis."
                        }
                    }
                }
            };

            await client.VerifyEmailAddressAsync(new VerifyEmailAddressRequest
            {
                EmailAddress = "sender@email.com"
            });

            var response = await client.SendEmailAsync(request);

            Assert.True(response.HttpStatusCode == HttpStatusCode.OK);
            Assert.NotEmpty(response.MessageId);
        }
    }
}