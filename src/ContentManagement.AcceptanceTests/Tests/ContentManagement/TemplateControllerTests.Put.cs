using cCoder.Data.Models.CMS;
using FluentAssertions;
using Xunit;


namespace Web.AcceptanceTests.Tests.ContentManagement;

public sealed partial class TemplateControllerTests
{
    [Fact]
    public async Task Put_UpdatesTemplate()
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
        Template expectedTemplate = new() { Description = "Updated template" };

        // When
        await UpdateTemplateAsync(
            createdTemplate.Id,
            new
            {
                id = createdTemplate.Id,
                appId = 1,
                name = Unique("UpdatedTemplate"),
                description = "Updated template",
                resourceKey = "Default",
                rawString = "<html><body><p>Updated</p></body></html>",
            });
        Template actualTemplate = await GetTemplateAsync(createdTemplate.Id);

        // Then
        actualTemplate.Should().NotBeNull();
        actualTemplate!.Description.Should().Be(expectedTemplate.Description);

        await DeleteTemplateAsync(createdTemplate.Id);
    }
}






