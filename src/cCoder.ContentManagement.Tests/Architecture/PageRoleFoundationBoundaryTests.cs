// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Brokers.Storages;
using cCoder.ContentManagement.Services.Foundations.Storages;
using FluentAssertions;
using Xunit;

namespace cCoder.ContentManagement.Tests.Architecture;

public sealed partial class PageRoleFoundationBoundaryTests
{
    [Fact]
    public void PageRoleService_WhenConstructed_ShouldOnlyConsumePageRoleBroker()
    {
        // Given
        Type serviceType = typeof(PageRoleService);

        // When
        Type[] dependencyTypes = serviceType
            .GetConstructors()
            .Single()
            .GetParameters()
            .Select(selector: parameter => parameter.ParameterType)
            .ToArray();

        // Then
        Assert.Equal(
            expected: new[] { typeof(IPageRoleBroker) },
            actual: dependencyTypes);
    }
}