// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Reflection;
using cCoder.ContentManagement.Services.Foundations.Exports;
using FluentAssertions;
using Xunit;

namespace cCoder.ContentManagement.Tests.Architecture;

public sealed class PackageExportBoundaryArchitectureTests
{
    [Fact]
    public void PackageExportFoundation_WhenComposed_HasOneDataBroker()
    {
        // Given / When
        string[] brokerContracts = typeof(PackageExportService)
            .GetConstructors(
                BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)
            .Single()
            .GetParameters()
            .Select(parameter => parameter.ParameterType.Name)
            .Where(name => name.EndsWith("Broker", StringComparison.Ordinal))
            .Where(name => name != "IJsonBroker")
            .ToArray();

        // Then
        brokerContracts.Should().Equal("IPackageExportBroker");
    }
}