// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Packaging;
using cCoder.Data.Models.Security;
using ComponentRenderParams = cCoder.ContentManagement.Models.ComponentRenderParams;
using Config = cCoder.ContentManagement.Models.Config;
using PageRenderParams = cCoder.ContentManagement.Models.PageRenderParams;
using PageRoleInfo = cCoder.ContentManagement.Models.PageRoleInfo;
using RenderParams = cCoder.ContentManagement.Models.RenderParams;
using RenderResult = cCoder.ContentManagement.Models.RenderResult;
using TemplateRenderParams = cCoder.ContentManagement.Models.TemplateRenderParams;
using System.Security;



using Moq;
using Xunit;


namespace cCoder.Core.Services.Tests.CMS.Processings;

public partial class LayoutProcessingServiceTests
{
    [Fact]
    public async Task ShouldDelegateToFoundationServiceWhenAddAsync()
    {
        // Given
        Layout layout = CreateRandomLayout();

        layoutServiceMock.Setup(expression: x => x.AddLayoutAsync(newLayout: layout))
            .ReturnsAsync(value: layout);

        // When
        Layout result = await layoutProcessingService.AddLayoutAsync(newLayout: layout);

        // Then
        Assert.Same(expected: layout, actual: result);
        layoutServiceMock.Verify(expression: x => x.AddLayoutAsync(newLayout: layout), times: Times.Once);
    }

    [Fact]
    public async Task ShouldPropagateSecurityExceptionWhenUserLacksCreatePrivilegeForAddAsync()
    {
        // Given
        Layout layout = CreateRandomLayout();

        layoutServiceMock
            .Setup(expression: x => x.AddLayoutAsync(newLayout: layout))
            .ThrowsAsync(exception: new SecurityException(message: "Access Denied!"));

        // When

        await Assert.ThrowsAsync<SecurityException>(testCode: async () =>
            await layoutProcessingService.AddLayoutAsync(newLayout: layout)
        );

        // Then
    }

}