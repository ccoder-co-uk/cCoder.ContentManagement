// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Exposures.Controllers;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using System.Reflection;
using Xunit;

namespace cCoder.ContentManagement.Tests.Exposures;

public sealed partial class ContentManagementApiSurfaceTests
{
    private static readonly Type[] ControllerTypes =
    [
        typeof(AppController),
        typeof(AppCultureController),
        typeof(CommonObjectController),
        typeof(ComponentController),
        typeof(ContentController),
        typeof(CultureController),
        typeof(LayoutController),
        typeof(PageController),
        typeof(PageInfoController),
        typeof(PageRenderCacheController),
        typeof(PageRoleController),
        typeof(RenderController),
        typeof(ResourceController),
        typeof(ScriptController),
        typeof(SubmissionController),
        typeof(TemplateController)
    ];

    private static readonly Type[] FormerPatchControllerTypes =
    [
        typeof(AppController),
        typeof(CommonObjectController),
        typeof(ComponentController),
        typeof(ContentController),
        typeof(CultureController),
        typeof(LayoutController),
        typeof(PageController),
        typeof(PageInfoController),
        typeof(PageRenderCacheController),
        typeof(ResourceController),
        typeof(ScriptController),
        typeof(SubmissionController),
        typeof(TemplateController)
    ];

    [Fact]
    public void Controllers_WhenInspected_DoNotExposePatchOrMergeOperations()
    {
        // Given
        MethodInfo[] controllerMethods = ControllerTypes
            .SelectMany(selector: type => type.GetMethods(bindingAttr: BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly))
            .ToArray();

        // When
        MethodInfo[] patchOrMergeMethods = controllerMethods
            .Where(predicate: method => method
                .GetCustomAttributes<AcceptVerbsAttribute>()
                .SelectMany(selector: attribute => attribute.HttpMethods)
                .Any(predicate: verb => verb.Equals(value: "PATCH", comparisonType: StringComparison.OrdinalIgnoreCase)
                    || verb.Equals(value: "MERGE", comparisonType: StringComparison.OrdinalIgnoreCase)))
            .ToArray();

        // Then
        patchOrMergeMethods.Should()
            .BeEmpty();
    }

    [Fact]
    public void EntityControllers_WhenInspected_ContinueToExposePutOperations()
    {
        // Given
        Type[] entityControllers = FormerPatchControllerTypes;

        // When
        Type[] controllersWithoutPut = entityControllers
            .Where(predicate: type => !type
                .GetMethods(bindingAttr: BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly)
                .Any(predicate: method => method.GetCustomAttribute<HttpPutAttribute>() is not null))
            .ToArray();

        // Then
        controllersWithoutPut.Should()
            .BeEmpty();
    }

    [Fact]
    public void Controllers_WhenInspected_DoNotExposePerEntityGetMetadataActions()
    {
        // Given
        MethodInfo[] controllerMethods = ControllerTypes
            .SelectMany(selector: type => type.GetMethods(bindingAttr: BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly))
            .ToArray();

        // When
        MethodInfo[] metadataActions = controllerMethods
            .Where(predicate: method => method.Name.Equals(value: "GetMetadata", comparisonType: StringComparison.Ordinal))
            .ToArray();

        // Then
        metadataActions.Should()
            .BeEmpty();
    }
}