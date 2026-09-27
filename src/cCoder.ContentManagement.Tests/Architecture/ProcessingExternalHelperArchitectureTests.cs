// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Runtime.CompilerServices;
using System.Text.Json;
using FluentAssertions;
using Xunit;

namespace cCoder.ContentManagement.Tests.Architecture;

public sealed partial class ProcessingExternalHelperArchitectureTests
{
    private static readonly HashSet<string> ExternalHelperTypes =
        new(StringComparer.Ordinal)
        {
            "System.Linq.Enumerable",
            "System.Math",
            "System.MemoryExtensions",
            "System.Object",
            "System.Type",
            "System.Text.StringBuilder"
        };

    [Fact]
    public void ProcessingServices_WhenModelled_DoNotCallExternalHelpers()
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
        string[] invalidCalls = model.RootElement
            .GetProperty(propertyName: "Classes")
            .EnumerateArray()
            .Where(predicate: type =>
                string.Equals(
                    a: type
                        .GetProperty(propertyName: "StandardElementType")
                        .GetString(),
                    b: "ProcessingService",
                    comparisonType: StringComparison.Ordinal))
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

        // Then
        invalidCalls.Should()
            .BeEmpty(
                because: "processing logic can express these operations directly without external helper dependencies");
    }

    private static string GetThisFilePath(
        [CallerFilePath] string filePath = "") =>
        filePath;
}