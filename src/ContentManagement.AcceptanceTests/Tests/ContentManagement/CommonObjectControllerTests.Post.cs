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
    public async Task Post_CreatesCommonObject()
    {
        // Given
        string name = Unique(prefix: "CommonObject");
        CommonObject expectedCommonObject = new() { Name = name };

        // When

        CommonObject createdCommonObject = await CreateCommonObjectAsync(payload: new
        {
            name,
            description = "Acceptance common object",
            version = 1,
            key = Unique(prefix: "key"),
            type = "Acceptance/Test",
            json = "{\"enabled\":true}",
            culture = string.Empty,
        });

        CommonObject actualCommonObject = await GetCommonObjectAsync(id: createdCommonObject.Id);

        // Then

        actualCommonObject.Should()
            .NotBeNull();

        actualCommonObject!.Name.Should()
            .Be(expected: expectedCommonObject.Name);

        await DeleteCommonObjectAsync(id: createdCommonObject.Id);
        await Teardown(ids: createdCommonObject.Id);
    }
}