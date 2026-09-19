// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Brokers;
using FluentAssertions;
using Xunit;

namespace cCoder.ContentManagement.Tests.Brokers;

public sealed partial class FingerprintBrokerTests
{
    [Fact]
    public void Compute_WhenSerializedValueProvided_ReturnsDeterministicHash()
    {
        // Given
        JsonBroker jsonBroker = new();
        FingerprintBroker fingerprintBroker = new();
        object value = new { Id = 7, Name = "Alpha" };

        // When
        string result = fingerprintBroker.Compute(
            value: jsonBroker.Serialize(value: value));

        // Then
        result.Should()
            .Be(expected: "D26E0620D9B69E0CB6BF80C0717522796D97B3A02276009CBC742A36EE3439D7");
    }
}