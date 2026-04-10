using cCoder.Data.Models.CMS;
using FluentAssertions;
using Xunit;


namespace Web.AcceptanceTests.Tests.ContentManagement;

public sealed partial class ComponentControllerTests
{
    [Fact]
    public async Task Delete_RemovesComponent()
    {
        // Given
        Component createdComponent = await CreateComponentAsync(
            new
            {
                appId = 1,
                name = Unique("Component"),
                description = "Acceptance component",
                resourceKey = "Default",
                content = "<div>Hello component</div>",
                script = "console.log('component');",
                key = "Acceptance",
            });
        int actualReadStatusCode;

        // When
        int actualStatusCode = await DeleteComponentAsync(createdComponent.Id);
        actualReadStatusCode = await GetComponentStatusCodeAsync(createdComponent.Id);

        // Then
        actualStatusCode.Should().Be(200);
        actualReadStatusCode.Should().Be(404);
    }
}





