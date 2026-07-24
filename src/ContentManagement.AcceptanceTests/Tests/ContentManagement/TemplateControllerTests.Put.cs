// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

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
payload: new
{
    appId = 1,
    name = Unique(prefix: "Template"),
    description = "Acceptance template",
    resourceKey = "Default",
    rawString = "<html><body><h1>[model[title]]</h1></body></html>",
});

        Template expectedTemplate = new() { Description = "Updated template" };

        // When

        await UpdateTemplateAsync(
id: createdTemplate.Id,
payload: new
{
    id = createdTemplate.Id,
    appId = 1,
    name = Unique(prefix: "UpdatedTemplate"),
    description = "Updated template",
    resourceKey = "Default",
    rawString = "<html><body><p>Updated</p></body></html>",
});

        Template actualTemplate = await GetTemplateAsync(id: createdTemplate.Id);

        // Then

        actualTemplate.Should()
            .NotBeNull();

        actualTemplate!.Description.Should()
            .Be(expected: expectedTemplate.Description);

        await DeleteTemplateAsync(id: createdTemplate.Id);
    }
}