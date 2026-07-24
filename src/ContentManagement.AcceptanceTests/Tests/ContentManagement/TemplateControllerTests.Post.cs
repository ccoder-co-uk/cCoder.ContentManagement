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
    public async Task Post_CreatesTemplate()
    {
        // Given
        string name = Unique(prefix: "Template");
        Template expectedTemplate = new() { Name = name };

        // When

        Template createdTemplate = await CreateTemplateAsync(
payload: new
{
    appId = 1,
    name,
    description = "Acceptance template",
    resourceKey = "Default",
    rawString = "<html><body><h1>[model[title]]</h1></body></html>",
});

        Template actualTemplate = await GetTemplateAsync(id: createdTemplate.Id);

        // Then

        actualTemplate.Should()
            .NotBeNull();

        actualTemplate!.Name.Should()
            .Be(expected: expectedTemplate.Name);

        await DeleteTemplateAsync(id: createdTemplate.Id);
    }
}