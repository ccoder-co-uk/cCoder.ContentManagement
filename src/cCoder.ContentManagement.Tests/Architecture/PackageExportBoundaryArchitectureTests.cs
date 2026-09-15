// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Reflection;
using cCoder.ContentManagement.Services.Foundations.Exports;
using FluentAssertions;
using Xunit;

namespace cCoder.ContentManagement.Tests.Architecture;

public sealed partial class PackageExportBoundaryArchitectureTests
{
    [Fact]
    public void PackageExportFoundation_WhenComposed_HasOneDataBroker()
    {
        // Given
        Type packageExportServiceType = typeof(PackageExportService);

        // When
        string[] brokerContracts = packageExportServiceType
            .GetConstructors(
                bindingAttr: BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)
            .Single()
            .GetParameters()
            .Select(selector: parameter => parameter.ParameterType.Name)
            .Where(predicate: name => name.EndsWith(value: "Broker", comparisonType: StringComparison.Ordinal))
            .Where(predicate: name => name != "IJsonBroker")
            .ToArray();

        // Then
        brokerContracts.Should()
            .Equal(expected: "IPackageExportBroker");
    }
}