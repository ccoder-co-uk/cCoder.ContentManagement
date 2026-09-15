// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Exposures;
using FluentAssertions;
using Xunit;

namespace cCoder.ContentManagement.Tests.Architecture;

public sealed class LegacyPackageCrudArchitectureTests
{
    [Theory]
    [InlineData("cCoder.ContentManagement.Services.Orchestrations.PackageOrchestrationService")]
    [InlineData("cCoder.ContentManagement.Services.Orchestrations.PackageItemOrchestrationService")]
    public void ContentManagementAssembly_WhenInspected_DoesNotOwnPackagingCrud(
        string typeName)
    {
        // Given / When
        Type legacyType = typeof(IContentManagementPackageManager)
            .Assembly
            .GetType(name: typeName);

        // Then
        legacyType.Should().BeNull();
    }
}