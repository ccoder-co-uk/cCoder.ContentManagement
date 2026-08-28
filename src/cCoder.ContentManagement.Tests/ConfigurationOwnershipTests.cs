// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Models;
using cCoder.Data;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace cCoder.ContentManagement.Tests;

public sealed partial class ConfigurationOwnershipTests
{
    [Fact]
    public void ContentManagementConfiguration_ShouldNotOwnPersistenceConfiguration()
    {
        // Given
        Type configurationType = typeof(ContentManagementConfiguration);

        // When
        string[] propertyNames = configurationType
            .GetProperties()
            .Select(selector: property => property.Name)
            .ToArray();

        // Then
        propertyNames.Should()
            .NotContain(unexpected: [
                "ConnectionString",
                "DebugInfo",
                "LogSQL"]);
    }

    [Fact]
    public void AddContentManagementWeb_ShouldNotRegisterCoreDataServices()
    {
        // Given
        IServiceCollection services = new ServiceCollection();
        ContentManagementConfiguration configuration = new();

        typeof(ContentManagementConfiguration)
            .GetProperty(name: "ConnectionString")
            ?.SetValue(obj: configuration, value: "Server=(local);");

        // When
        services.AddContentManagementWeb(configuration: configuration);

        // Then
        services.Should()
            .NotContain(predicate: descriptor =>
                descriptor.ServiceType == typeof(CoreDataContext));
    }
}