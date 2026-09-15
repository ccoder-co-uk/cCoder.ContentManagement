// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Api.OData;
using cCoder.ContentManagement.Models;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Xunit;

namespace cCoder.ContentManagement.Tests.Exposures.OData;

public sealed class BadRequestResultTests
{
    [Fact]
    public void BadRequestResult_WhenModelStateIsInvalid_ReturnsStructuredErrors()
    {
        // Given
        ModelStateDictionary modelState = new();
        modelState.AddModelError(key: "Name", errorMessage: "Name is required");

        // When
        BadRequestResult result = new(modelState: modelState);

        // Then
        result.Value.Should()
            .BeEquivalentTo(
                expectation:
                new ModelStateError[]
                {
                    new()
                    {
                        Key = "Name",
                        Errors = ["Name is required - "]
                    }
                });
    }
}