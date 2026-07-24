// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models;
using FluentAssertions;
using Xunit;


namespace Web.AcceptanceTests.Tests.ContentManagement;

public sealed partial class CommonObjectControllerTests
{
    [Fact]
    public async Task Import_CreatesCommonObjects()
    {
        // Given
        string name = Unique(prefix: "ImportedCommonObject");
        string key = Unique(prefix: "key");

        // When

        await ImportCommonObjectsAsync(payload: new
        {
            value = new object[]
            {
                new
                {
                    name,
                    description = "Imported common object",
                    version = 1,
                    key,
                    type = "Acceptance/Test",
                    json = "{\"enabled\":true}",
                    culture = string.Empty,
                },
            },
        });

        IReadOnlyList<CommonObject> actualCommonObjects = await FilterCommonObjectsByKeyAsync(key: key);

        // Then

        actualCommonObjects.Select(selector: item => item.Name)
            .Should()
            .Contain(expected: name);
    }
}