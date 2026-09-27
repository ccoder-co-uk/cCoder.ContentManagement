// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Exposures.Controllers;
using FluentAssertions;
using Xunit;

namespace cCoder.ContentManagement.Tests.Architecture;

public sealed partial class AppPageOrderArchitectureTests
{
    [Fact]
    public void AppController_WhenInspected_ShouldNotExposePageOrderAction()
    {
        // Given
        Type controllerType = typeof(AppController);

        // When
        bool exposesPageOrderAction = controllerType
            .GetMethods()
            .Any(predicate: method => method.Name.Contains(
                value: "UpdatePageOrder",
                comparisonType: StringComparison.Ordinal));

        // Then
        exposesPageOrderAction.Should()
            .BeFalse(
                because: "page order is part of the normal page update lifecycle");
    }
}