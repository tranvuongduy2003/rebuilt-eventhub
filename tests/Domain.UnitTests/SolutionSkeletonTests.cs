using FluentAssertions;
using Solution.Application.Abstractions.Email;
using Solution.Application.Abstractions.Messaging;
using Solution.Application.Abstractions.Payments;
using Solution.Application.Abstractions.Storage;

namespace Solution.Domain.UnitTests;

public class SolutionSkeletonTests
{
    [Theory]
    [InlineData(typeof(Solution.Domain.Events.ModuleMarker))]
    [InlineData(typeof(Solution.Domain.Orders.ModuleMarker))]
    [InlineData(typeof(Solution.Domain.Payments.ModuleMarker))]
    [InlineData(typeof(Solution.Domain.Tickets.ModuleMarker))]
    public void BoundedContextModules_ArePresent(Type moduleMarkerType)
    {
        moduleMarkerType.Assembly.Should().BeSameAs(Solution.Domain.AssemblyReference.Assembly);
    }

    [Fact]
    public void ApplicationPorts_AreDefinedForInfrastructureAdapters()
    {
        typeof(IObjectStorage).Assembly.Should().BeSameAs(Solution.Application.AssemblyReference.Assembly);
        typeof(IIntegrationEventPublisher).Assembly.Should().BeSameAs(Solution.Application.AssemblyReference.Assembly);
        typeof(IEmailSender).Assembly.Should().BeSameAs(Solution.Application.AssemblyReference.Assembly);
        typeof(IPaymentGateway).Assembly.Should().BeSameAs(Solution.Application.AssemblyReference.Assembly);
    }
}
