// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.CodeAnalysis.Exposures;
using cCoder.ContentManagement.Brokers;
using FluentAssertions;
using Xunit;

namespace cCoder.ContentManagement.Tests.Architecture;

public sealed class UtilityBrokerArchitectureTests
{
    [Theory]
    [InlineData(typeof(JsonBroker))]
    [InlineData(typeof(RegularExpressionBroker))]
    public void StatelessCrossCuttingBroker_WhenDeclared_ImplementsUtilityMarkerDirectly(
        Type brokerType)
    {
        // Given / When
        Type[] directContracts = brokerType.GetInterfaces()
            .Except(brokerType.BaseType?.GetInterfaces() ?? [])
            .ToArray();

        // Then
        directContracts.Should().Contain(typeof(IUtilityBroker));
    }
}