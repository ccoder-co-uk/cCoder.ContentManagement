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
    public async Task Render_ReturnsRenderedTemplateContent()
    {
        // Given
        string templateName = Unique(prefix: "Template");

        Template createdTemplate = await CreateTemplateAsync(
payload: new
{
    appId = 1,
    name = templateName,
    description = "Acceptance template",
    resourceKey = "Default",
    rawString = "<html><body><h1>[model[title]]</h1></body></html>",
});

        await UpdateTemplateAsync(
id: createdTemplate.Id,
payload: new
{
    id = createdTemplate.Id,
    appId = 1,
    name = templateName,
    description = "Updated acceptance template",
    resourceKey = "Default",
    rawString = "<html><body><h1>[model[title]]</h1><p>[model[body]]</p></body></html>",
});

        // When
        string actualRender = await RenderTemplateAsync(name: templateName, modelJson: """{"title":"Acceptance","body":"Rendered"}""");

        // Then

        actualRender.Should()
            .Contain(expected: "Acceptance");

        actualRender.Should()
            .Contain(expected: "Rendered");

        (int actualPdfStatusCode, string actualPdfMediaType) = await ConvertHtmlToPdfAsync(name: "acceptance", html: "<html><body><p>pdf</p></body></html>");

        actualPdfStatusCode.Should()
            .Be(expected: 200);

        actualPdfMediaType.Should()
            .Be(expected: "application/pdf");

        await DeleteTemplateAsync(id: createdTemplate.Id);
    }
}