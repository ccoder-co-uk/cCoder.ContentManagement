// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Runtime.CompilerServices;
using System.Text.Json;
using FluentAssertions;
using Xunit;

namespace cCoder.ContentManagement.Tests.Architecture;

public sealed partial class FoundationExternalHelperArchitectureTests
{
    private static readonly HashSet<string> ExternalHelperTypes =
        new(StringComparer.Ordinal)
        {
            "System.Linq.Enumerable",
            "System.Math",
            "System.MemoryExtensions",
            "System.Object",
            "System.Runtime.InteropServices.CollectionsMarshal",
            "System.Text.StringBuilder",
            "System.Type"
        };

    private static readonly HashSet<string> ExternalOperationTypes =
        new(StringComparer.Ordinal)
        {
            "System.Linq.Enumerable",
            "System.Math",
            "System.MemoryExtensions",
            "System.Runtime.InteropServices.CollectionsMarshal",
            "System.Text.StringBuilder"
        };

    [Fact]
    public void FoundationServices_WhenModelled_DoNotCallExternalHelpers()
    {
        // Given
        string modelPath = Path.GetFullPath(
            path: Path.Combine(
                paths:
                [
                    Path.GetDirectoryName(path: GetThisFilePath()),
                    "..",
                    "..",
                    "cCoder.ContentManagement",
                    "project.stxjson"
                ]));

        using JsonDocument model = JsonDocument.Parse(
            json: File.ReadAllText(path: modelPath));

        // When
        JsonElement[] foundationTypes = model.RootElement
            .GetProperty(propertyName: "Classes")
            .EnumerateArray()
            .Where(predicate: type =>
                string.Equals(
                    a: type
                        .GetProperty(propertyName: "StandardElementType")
                        .GetString(),
                    b: "FoundationService",
                    comparisonType: StringComparison.Ordinal))
            .ToArray();

        HashSet<string> foundationTypeNames = foundationTypes
            .Select(selector: type => type
                .GetProperty(propertyName: "Name")
                .GetString())
            .ToHashSet(comparer: StringComparer.Ordinal);

        string[] invalidCalls = foundationTypes
            .SelectMany(selector: type => type
                .GetProperty(propertyName: "Methods")
                .EnumerateArray()
                .SelectMany(selector: method => method
                    .GetProperty(propertyName: "Calls")
                    .EnumerateArray()
                    .Where(predicate: call => ExternalHelperTypes.Contains(
                        item: call
                            .GetProperty(propertyName: "TypeName")
                            .GetString()))
                    .Select(selector: call =>
                        $"{type
                            .GetProperty(propertyName: "Name")
                            .GetString()} -> "
                        + call
                            .GetProperty(propertyName: "MethodId")
                            .GetString())))
            .Distinct(comparer: StringComparer.Ordinal)
            .OrderBy(keySelector: finding => finding, comparer: StringComparer.Ordinal)
            .ToArray();

        string[] invalidLinks = model.RootElement
            .GetProperty(propertyName: "Links")
            .EnumerateArray()
            .Where(predicate: link => foundationTypeNames.Contains(
                item: link
                    .GetProperty(propertyName: "FromType")
                    .GetString())
                && ExternalOperationTypes.Contains(
                    item: link
                        .GetProperty(propertyName: "ToType")
                        .GetString()))
            .Select(selector: link =>
                $"{link
                    .GetProperty(propertyName: "FromType")
                    .GetString()} -> "
                + link
                    .GetProperty(propertyName: "ToType")
                    .GetString())
            .Distinct(comparer: StringComparer.Ordinal)
            .OrderBy(keySelector: finding => finding, comparer: StringComparer.Ordinal)
            .ToArray();

        // Then
        invalidCalls
            .Concat(second: invalidLinks)
            .Should()
            .BeEmpty(
                because: "foundation logic can express these operations directly or delegate external behavior to its broker");
    }

    private static string GetThisFilePath(
        [CallerFilePath] string filePath = "") =>
        filePath;
}