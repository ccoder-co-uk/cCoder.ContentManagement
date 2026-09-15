// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Reflection;
using cCoder.ContentManagement.Dependencies.Caching;
using cCoder.ContentManagement.Exposures.Caching;
using FluentAssertions;
using Xunit;

namespace cCoder.ContentManagement.Tests.Architecture;

public sealed partial class MetadataCacheBoundaryArchitectureTests
{
    [Fact]
    public void MetadataCacheDependency_WhenComposed_DoesNotDependOnBroker()
    {
        // Given
        Type dependencyType = typeof(MetadataCacheDependency);

        // When
        Type[] dependencyTypes = dependencyType
            .GetConstructors(
                bindingAttr: BindingFlags.Instance
                    | BindingFlags.NonPublic
                    | BindingFlags.Public)
            .Single()
            .GetParameters()
            .Select(selector: parameter => parameter.ParameterType)
            .ToArray();

        // Then
        dependencyTypes.Should()
            .Contain(predicate: dependencyType =>
                dependencyType == typeof(ICommonObjectCache));

        dependencyTypes.Should()
            .NotContain(predicate: dependencyType =>
                dependencyType.Name.EndsWith(
                    value: "Broker",
                    comparisonType: StringComparison.Ordinal));
    }
}