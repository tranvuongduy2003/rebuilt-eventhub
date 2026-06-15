using System.Net;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Solution.Api.IntegrationTests.Integration;
using Solution.Application.Abstractions.Email;
using Solution.Application.Abstractions.Messaging;
using Solution.Application.Abstractions.Payments;
using Solution.Application.Abstractions.Storage;
using Solution.Testing.Common.Fixtures;

namespace Solution.Api.IntegrationTests.Infrastructure;

[Collection(IntegrationTestCollection.Name)]
public sealed class FoundationSmokeTests(IntegrationTestFixture fixture)
{
    private readonly HttpClient _client = fixture.Factory.CreateClient(
        new WebApplicationFactoryClientOptions { HandleCookies = true });

    [Fact]
    public async Task HealthEndpoint_ReturnsOk()
    {
        var response = await _client.GetAsync("/api/health");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public void InfrastructurePorts_AreRegistered()
    {
        using var scope = fixture.Factory.Services.CreateScope();
        var services = scope.ServiceProvider;

        services.GetService<IObjectStorage>().Should().NotBeNull();
        services.GetService<IIntegrationEventPublisher>().Should().NotBeNull();
        services.GetService<IEmailSender>().Should().NotBeNull();
        services.GetService<IPaymentGateway>().Should().NotBeNull();
    }
}
