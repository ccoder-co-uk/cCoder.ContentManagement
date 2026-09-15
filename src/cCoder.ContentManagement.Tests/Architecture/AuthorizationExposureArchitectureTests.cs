// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Exposures;
using cCoder.CodeAnalysis.Exposures;
using cCoder.ContentManagement.Brokers;
using FluentAssertions;
using Xunit;

namespace cCoder.ContentManagement.Tests.Architecture;

public sealed partial class AuthorizationExposureArchitectureTests
{
    [Fact]
    public void ContentManagementAssembly_WhenInspected_DoesNotExposeAuthorizationManager()
    {
        // Given
        Type contentManagementPackageManagerType = typeof(IContentManagementPackageManager);

        // When
        Type authorizationManager = contentManagementPackageManagerType
            .Assembly
            .GetType(name: "cCoder.ContentManagement.Exposures.AuthorizationManager");

        // Then
        authorizationManager
            .Should()
            .BeNull();
    }

    [Fact]
    public void AuthorizationBrokerContract_WhenInspected_IsInternalAndNotUtilityInfrastructure()
    {
        // Given
        Type utilityBrokerType = typeof(IUtilityBroker);

        // When
        Type authorizationBrokerContract = typeof(IAuthorizationBroker);
        Type[] inheritedContracts = authorizationBrokerContract.GetInterfaces();

        // Then
        authorizationBrokerContract.IsPublic
            .Should()
            .BeFalse();

        inheritedContracts
            .Should()
            .NotContain(unexpected: utilityBrokerType);
    }
}