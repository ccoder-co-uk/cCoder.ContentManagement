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
            new
            {
                appId = 1,
                name = Unique("Template"),
                description = "Acceptance template",
                resourceKey = "Default",
                rawString = "<html><body><h1>[model[title]]</h1></body></html>",
            });

        // When
        int actualStatusCode = await DeleteTemplateAsync(createdTemplate.Id);
        int actualReadStatusCode = await GetTemplateStatusCodeAsync(createdTemplate.Id);

        // Then
        actualStatusCode.Should().Be(200);
        actualReadStatusCode.Should().Be(404);
    }
}





