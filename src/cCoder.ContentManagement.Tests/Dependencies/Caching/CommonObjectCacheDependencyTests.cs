// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Dependencies.Caching;
using cCoder.ContentManagement.Models;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace cCoder.ContentManagement.Tests.Caching;

public partial class CommonObjectCacheTests
{
    [Fact]
    public void ConstructorShouldAcceptUnsetCacheExpiry()
    {
        // Given
        ContentManagementConfiguration configuration = new();
        Mock<IServiceScopeFactory> serviceScopeFactory = new();
        Mock<ILogger<CommonObjectCacheDependency>> logger = new();

        // When
        using CommonObjectCacheDependency dependency = new(
            configuration,
            serviceScopeFactory.Object,
            logger.Object);

        // Then
        dependency
            .Should()
            .NotBeNull();
    }
}