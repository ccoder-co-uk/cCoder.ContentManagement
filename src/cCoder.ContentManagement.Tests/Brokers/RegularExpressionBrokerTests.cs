// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Brokers;
using FluentAssertions;
using Xunit;

namespace cCoder.ContentManagement.Tests.Brokers;

public sealed partial class RegularExpressionBrokerTests
{
    [Fact]
    public void Replace_WhenNamedAndNumberedGroupsAreUsed_PreservesMatchSemantics()
    {
        // Given
        RegularExpressionBroker broker = new();

        // When
        string result = broker.Replace(
            input: "before [execute]return 42;[/execute] after",
            pattern: "\\[execute\\](?<code>.*?)\\[/execute\\]",
            evaluator: (value, groups) =>
                $"<{groups["code"]}|{groups["1"]}|{value}>");

        // Then
        result.Should()
            .Be(
            expected: "before <return 42;|return 42;|[execute]return 42;[/execute]> after");
    }

    [Fact]
    public void Match_WhenStartIndexIsProvided_ReturnsTheNextMatch()
    {
        // Given
        RegularExpressionBroker broker = new();

        // When
        bool result = broker.TryMatch(
            input: "<style>first</style><script>second</script>",
            pattern: "<(?<tag>script|style)\\b",
            startIndex: 7,
            index: out int index,
            groups: out IReadOnlyDictionary<string, string> groups);

        // Then
        result.Should()
            .BeTrue();

        index.Should()
            .Be(expected: 20);

        groups["tag"].Should()
            .Be(expected: "script");
    }

    [Fact]
    public void Matches_WhenMultipleMatchesExist_ReturnsThemInSourceOrder()
    {
        // Given
        RegularExpressionBroker broker = new();

        // When
        List<string> results = [];

        broker.ForEachMatch(
            input: "[item[a]][item[b]]",
            pattern: "\\[item\\[(?<name>[^\\]]+)\\]\\]",
            action: (value, groups) => results.Add(item: groups["name"]));

        // Then
        results.Should()
            .Equal(expected: ["a", "b"]);
    }
}