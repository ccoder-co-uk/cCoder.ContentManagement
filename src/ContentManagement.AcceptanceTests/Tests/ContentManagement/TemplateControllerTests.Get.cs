using cCoder.Data.Models.CMS;
using FluentAssertions;
using Xunit;


namespace Web.AcceptanceTests.Tests.ContentManagement;

public sealed partial class TemplateControllerTests
{
    [Fact]
    public async Task Get_ReturnsCreatedTemplate()
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
        Template expectedTemplate = new() { Id = createdTemplate.Id };

        // When
        Template actualTemplate = await GetTemplateAsync(createdTemplate.Id);

        // Then
        actualTemplate.Should().NotBeNull();
        actualTemplate!.Id.Should().Be(expectedTemplate.Id);

        await DeleteTemplateAsync(createdTemplate.Id);
    }

    [Fact]
    public async Task GetCount_ReturnsNonNegativeCount()
    {
        // Given

        // When
        int actualCount = await GetTemplateCountAsync();

        // Then
        actualCount.Should().BeGreaterThanOrEqualTo(0);
    }
}






