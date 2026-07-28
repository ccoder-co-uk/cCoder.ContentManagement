// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Packaging;
using cCoder.Data.Models.Security;
using ComponentRenderParams = cCoder.ContentManagement.Models.ComponentRenderParams;
using Config = cCoder.ContentManagement.Models.ContentManagementConfiguration;
using PageRenderParams = cCoder.ContentManagement.Models.PageRenderParams;
using PageRoleInfo = cCoder.ContentManagement.Models.PageRoleInfo;
using RenderParams = cCoder.ContentManagement.Models.RenderParams;
using RenderResult = cCoder.ContentManagement.Models.RenderResult;
using TemplateRenderParams = cCoder.ContentManagement.Models.TemplateRenderParams;
using Moq;
using Xunit;


namespace cCoder.Core.Services.Tests.CMS.Processings;

public partial class CultureProcessingServiceTests
{
    [Fact]
    public async Task ShouldDelegateToFoundationServiceWhenDeleteAsync()
    {
        // Given
        Culture entity = CreateRandomCulture();
        var id = entity.Id;

        cultureServiceMock.Setup(expression: x => x.DeleteAsync(cultureId: id))
            .Returns(value: ValueTask.CompletedTask);

        // When
        await cultureProcessingService.DeleteAsync(cultureId: id);

        // Then
        cultureServiceMock.Verify(expression: x => x.DeleteAsync(cultureId: id), times: Times.Once);
        cultureServiceMock.VerifyNoOtherCalls();
    }

}