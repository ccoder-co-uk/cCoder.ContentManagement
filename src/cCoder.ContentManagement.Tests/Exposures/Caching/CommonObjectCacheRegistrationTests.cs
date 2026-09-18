// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Exposures.Caching;
using cCoder.ContentManagement.Models;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace cCoder.ContentManagement.Tests.Exposures.Caching;

public sealed partial class CommonObjectCacheRegistrationTests
{
    [Fact]
    public void AddContentManagementWeb_ShouldRegisterCommonObjectCacheAsScoped()
    {
        // Given
        ServiceCollection services = [];

        // When
        services.AddContentManagementWeb(
            configuration: new ContentManagementConfiguration());

        // Then
        ServiceDescriptor descriptor = services
            .Where(predicate: descriptor =>
                descriptor.ServiceType == typeof(ICommonObjectCache))
            .Should()
            .ContainSingle()
            .Which;

        descriptor.Lifetime.Should()
            .Be(expected: ServiceLifetime.Scoped);

        descriptor.ImplementationType.Should()
            .Be(expected: typeof(CommonObjectCacheManager));
    }
}