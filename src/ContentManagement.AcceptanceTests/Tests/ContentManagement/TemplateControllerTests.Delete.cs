// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.CMS;
using FluentAssertions;
using Xunit;


namespace Web.AcceptanceTests.Tests.ContentManagement;

public sealed partial class TemplateControllerTests
{
    [Fact]
    public async Task Delete_RemovesTemplate()
    {
        // Given
        Template createdTemplate = await CreateTemplateAsync(
payload: new
{
    appId = 1,
    name = Unique(prefix: "Template"),
    description = "Acceptance template",
    resourceKey = "Default",
    rawString = "<html><body><h1>[model[title]]</h1></body></html>",
});

        // When
        int actualStatusCode = await DeleteTemplateAsync(id: createdTemplate.Id);
        int actualReadStatusCode = await GetTemplateStatusCodeAsync(id: createdTemplate.Id);

        // Then

        actualStatusCode.Should()
            .Be(expected: 204);

        actualReadStatusCode.Should()
            .Be(expected: 404);
    }
}