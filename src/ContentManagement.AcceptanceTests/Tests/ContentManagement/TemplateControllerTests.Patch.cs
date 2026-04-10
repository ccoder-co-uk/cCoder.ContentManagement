using cCoder.Data.Models.CMS;
using FluentAssertions;
using Xunit;


namespace Web.AcceptanceTests.Tests.ContentManagement;

public sealed partial class TemplateControllerTests
{
    [Fact]
    public async Task Patch_UpdatesTemplateDescription()
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
        Template expectedTemplate = new() { Description = "Patched template" };

        // When
        await PatchTemplateAsync(createdTemplate.Id, new { description = "Patched template" });
        Template actualTemplate = await GetTemplateAsync(createdTemplate.Id);

        // Then
        actualTemplate.Should().NotBeNull();
        actualTemplate!.Description.Should().Be(expectedTemplate.Description);

        await DeleteTemplateAsync(createdTemplate.Id);
    }
}






