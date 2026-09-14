// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.CodeAnalysis.Exposures;
using cCoder.ContentManagement.Brokers.Loggings;
using FluentAssertions;
using Xunit;

namespace cCoder.Core.Services.Tests.CMS.Brokers.Loggings;

public sealed class LoggingBrokerBoundaryTests
{
    [Fact]
    public void LoggingBroker_WhenClassifiedAsUtility_ShouldCarryMarkerDirectly()
    {
        // Given
        Type implementation = typeof(LoggingBroker);
        Type contract = typeof(ILoggingBroker);

        // When
        Type[] directImplementationInterfaces = implementation
            .GetInterfaces()
            .Except(contract.GetInterfaces())
            .ToArray();

        // Then
        directImplementationInterfaces.Should().Contain(typeof(IUtilityBroker));
        contract.GetInterfaces().Should().NotContain(typeof(IUtilityBroker));
    }
}
