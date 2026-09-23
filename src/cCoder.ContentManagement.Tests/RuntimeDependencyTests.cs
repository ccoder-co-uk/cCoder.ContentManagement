// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Services.Foundations.Storages;
using Xunit;

namespace cCoder.ContentManagement.Tests;

public sealed partial class RuntimeDependencyTests
{
    [Fact]
    public void ContentManagementAssembly_WhenRuntimeDependenciesAreInspected_ReferencesContractsAndNotAnalyzer()
    {
        // Given
        string[] referencedAssemblies = typeof(AppService)
            .Assembly
            .GetReferencedAssemblies()
            .Select(selector: assemblyName => assemblyName.Name)
            .ToArray();

        // When
        bool referencesContracts = referencedAssemblies.Contains(
            value: "cCoder.CodeAnalysis.Contracts");

        bool referencesAnalyzer = referencedAssemblies.Contains(
            value: "cCoder.CodeAnalysis");

        // Then
        Assert.True(condition: referencesContracts);
        Assert.False(condition: referencesAnalyzer);
    }
}