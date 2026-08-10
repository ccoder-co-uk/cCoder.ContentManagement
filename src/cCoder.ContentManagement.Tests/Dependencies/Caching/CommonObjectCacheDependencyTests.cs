// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Brokers;
using cCoder.ContentManagement.Brokers.Storages;
using cCoder.ContentManagement.Dependencies.Caching;
using cCoder.ContentManagement.Models;
using cCoder.Data.Models;
using cCoder.Data.Models.CMS;
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

    [Fact]
    public void RefreshShouldSelectHighestVersionForEffectiveCacheIdentity()
    {
        // Given
        CommonObject latestCommonObject = new()
        {
            Id = 32555,
            Name = "AppManagement",
            Key = "Current",
            Type = "ContentManagement/Component",
            Version = 4,
            Json = "latest"
        };

        CommonObject legacyCommonObject = new()
        {
            Id = 12000,
            Name = "AppManagement",
            Key = "Legacy",
            Type = "ContentManagement/Component",
            Version = 2,
            Json = "legacy"
        };

        Component latestComponent = new()
        {
            Name = "AppManagement",
            Script = "corrected-script"
        };

        Component legacyComponent = new()
        {
            Name = "AppManagement",
            Script = "legacy-script"
        };

        Mock<ICommonObjectBroker> commonObjectBroker = new();
        Mock<IJsonBroker> jsonBroker = new();
        Mock<IServiceProvider> serviceProvider = new();
        Mock<IServiceScope> serviceScope = new();
        Mock<IServiceScopeFactory> serviceScopeFactory = new();
        Mock<ILogger<CommonObjectCacheDependency>> logger = new();

        commonObjectBroker
            .Setup(expression: broker => broker.GetLatestCommonObjectsPaged(
                pageSize: It.IsAny<int>()))
            .Returns(value: [latestCommonObject, legacyCommonObject]);

        jsonBroker
            .Setup(expression: broker => broker.ParseJson<Component>(
                json: latestCommonObject.Json))
            .Returns(value: latestComponent);

        jsonBroker
            .Setup(expression: broker => broker.ParseJson<Component>(
                json: legacyCommonObject.Json))
            .Returns(value: legacyComponent);

        serviceProvider
            .Setup(expression: provider => provider.GetService(
                serviceType: typeof(ICommonObjectBroker)))
            .Returns(value: commonObjectBroker.Object);

        serviceProvider
            .Setup(expression: provider => provider.GetService(
                serviceType: typeof(IJsonBroker)))
            .Returns(value: jsonBroker.Object);

        serviceScope
            .SetupGet(expression: scope => scope.ServiceProvider)
            .Returns(value: serviceProvider.Object);

        serviceScopeFactory
            .Setup(expression: factory => factory.CreateScope())
            .Returns(value: serviceScope.Object);

        using CommonObjectCacheDependency dependency = new(
            new ContentManagementConfiguration(),
            serviceScopeFactory.Object,
            logger.Object);

        // When
        dependency.Refresh();

        // Then
        dependency.GetLatestSet()
            .Should()
            .ContainSingle()
            .Which.Should()
            .BeSameAs(expected: latestCommonObject);

        dependency.Get<Component>(key: "component|appmanagement")
            .Should()
            .BeSameAs(expected: latestComponent);

        jsonBroker.Verify(
            expression: broker => broker.ParseJson<Component>(
                json: legacyCommonObject.Json),
            times: Times.Never);
    }
}