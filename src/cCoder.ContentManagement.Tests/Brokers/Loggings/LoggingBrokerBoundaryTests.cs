// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.CodeAnalysis.Exposures;
using cCoder.ContentManagement.Brokers.Loggings;
using FluentAssertions;
using Xunit;

namespace cCoder.Core.Services.Tests.CMS.Brokers.Loggings;

public sealed partial class LoggingBrokerBoundaryTests
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
            .Except(second: contract.GetInterfaces())
            .ToArray();

        // Then
        directImplementationInterfaces.Should()
            .Contain(expected: typeof(IUtilityBroker));

        contract.GetInterfaces()
            .Should()
            .NotContain(unexpected: typeof(IUtilityBroker));
    }
}