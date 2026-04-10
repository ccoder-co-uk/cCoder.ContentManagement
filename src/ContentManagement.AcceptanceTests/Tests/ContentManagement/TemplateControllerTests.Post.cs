using cCoder.Data.Models.CMS;
using FluentAssertions;
using Xunit;


namespace Web.AcceptanceTests.Tests.ContentManagement;

public sealed partial class TemplateControllerTests
{
    [Fact]
    public async Task Post_CreatesTemplate()
    {
        // Given
        string name = Unique("Template");
        Template expectedTemplate = new() { Name = name };

        // When
        Template createdTemplate = await CreateTemplateAsync(
            new
            {
                appId = 1,
                name,
                description = "Acceptance template",
                resourceKey = "Default",
                rawString = "<html><body><h1>[model[title]]</h1></body></html>",
            });
        Template actualTemplate = await GetTemplateAsync(createdTemplate.Id);

        // Then
        actualTemplate.Should().NotBeNull();
        actualTemplate!.Name.Should().Be(expectedTemplate.Name);

        await DeleteTemplateAsync(createdTemplate.Id);
    }
}






