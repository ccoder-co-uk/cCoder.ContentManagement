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
        string templateName = Unique("Template");
        Template createdTemplate = await CreateTemplateAsync(
            new
            {
                appId = 1,
                name = templateName,
                description = "Acceptance template",
                resourceKey = "Default",
                rawString = "<html><body><h1>[model[title]]</h1></body></html>",
            });

        await UpdateTemplateAsync(
            createdTemplate.Id,
            new
            {
                id = createdTemplate.Id,
                appId = 1,
                name = templateName,
                description = "Updated acceptance template",
                resourceKey = "Default",
                rawString = "<html><body><h1>[model[title]]</h1><p>[model[body]]</p></body></html>",
            });

        // When
        string actualRender = await RenderTemplateAsync(templateName, """{"title":"Acceptance","body":"Rendered"}""");

        // Then
        actualRender.Should().Contain("Acceptance");
        actualRender.Should().Contain("Rendered");

        (int actualPdfStatusCode, string actualPdfMediaType) = await ConvertHtmlToPdfAsync("acceptance", "<html><body><p>pdf</p></body></html>");
        actualPdfStatusCode.Should().Be(200);
        actualPdfMediaType.Should().Be("application/pdf");

        await DeleteTemplateAsync(createdTemplate.Id);
    }
}






