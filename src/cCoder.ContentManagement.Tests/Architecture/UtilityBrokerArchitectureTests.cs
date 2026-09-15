// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.CodeAnalysis.Exposures;
using cCoder.ContentManagement.Brokers;
using FluentAssertions;
using Xunit;

namespace cCoder.ContentManagement.Tests.Architecture;

public sealed partial class UtilityBrokerArchitectureTests
{
    [Theory]
    [InlineData(typeof(JsonBroker))]
    [InlineData(typeof(RegularExpressionBroker))]
    public void StatelessCrossCuttingBroker_WhenDeclared_ImplementsUtilityMarkerDirectly(
        Type brokerType)
    {
        // Given
        Type brokerBaseType = brokerType.BaseType;

        // When
        Type[] directContracts = brokerType.GetInterfaces()
            .Except(second: brokerBaseType?.GetInterfaces() ?? [])
            .ToArray();

        // Then
        directContracts.Should()
            .Contain(expected: typeof(IUtilityBroker));
    }
}