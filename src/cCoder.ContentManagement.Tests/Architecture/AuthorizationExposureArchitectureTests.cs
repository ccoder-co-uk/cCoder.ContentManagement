// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Exposures;
using cCoder.CodeAnalysis.Exposures;
using cCoder.ContentManagement.Brokers;
using FluentAssertions;
using Xunit;

namespace cCoder.ContentManagement.Tests.Architecture;

public sealed class AuthorizationExposureArchitectureTests
{
    [Fact]
    public void ContentManagementAssembly_WhenInspected_DoesNotExposeAuthorizationManager()
    {
        // Given / When
        Type authorizationManager = typeof(IContentManagementPackageManager)
            .Assembly
            .GetType("cCoder.ContentManagement.Exposures.AuthorizationManager");

        // Then
        authorizationManager.Should().BeNull();
    }

    [Fact]
    public void AuthorizationBrokerContract_WhenInspected_IsInternalAndNotUtilityInfrastructure()
    {
        // Given / When
        Type authorizationBrokerContract = typeof(IAuthorizationBroker);

        // Then
        authorizationBrokerContract.IsPublic.Should().BeFalse();
        authorizationBrokerContract.GetInterfaces().Should().NotContain(typeof(IUtilityBroker));
    }
}
