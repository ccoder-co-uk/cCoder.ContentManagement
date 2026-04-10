using cCoder.Data.Models;
using FluentAssertions;
using Xunit;


namespace Web.AcceptanceTests.Tests.ContentManagement;

public sealed partial class CommonObjectControllerTests
{
    [Fact]
    public async Task Post_CreatesCommonObject()
    {
        // Given
        string name = Unique("CommonObject");
        CommonObject expectedCommonObject = new() { Name = name };

        // When
        CommonObject createdCommonObject = await CreateCommonObjectAsync(new
        {
            name,
            description = "Acceptance common object",
            version = 1,
            key = Unique("key"),
            type = "Acceptance/Test",
            json = "{\"enabled\":true}",
            culture = string.Empty,
        });

        CommonObject actualCommonObject = await GetCommonObjectAsync(createdCommonObject.Id);

        // Then
        actualCommonObject.Should().NotBeNull();
        actualCommonObject!.Name.Should().Be(expectedCommonObject.Name);

        await DeleteCommonObjectAsync(createdCommonObject.Id);
        await Teardown(createdCommonObject.Id);
    }
}






