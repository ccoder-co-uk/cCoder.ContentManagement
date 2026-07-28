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
using FluentAssertions;
using Moq;
using Xunit;


namespace cCoder.Core.Services.Tests.CMS.Processings;

public partial class TemplateProcessingServiceTests
{
    [Fact]
    public async Task ShouldDelegateToFoundationServiceWhenUpdateAsync()
    {
        // Given
        Template entity = CreateRandomTemplate();

        templateServiceMock.Setup(expression: x => x.UpdateTemplateAsync(updatedTemplate: entity))
            .ReturnsAsync(value: entity);

        // When
        Template result = await templateProcessingService.UpdateTemplateAsync(updatedTemplate: entity);

        // Then

        result.Should()
            .BeSameAs(expected: entity);

        templateServiceMock.Verify(expression: x => x.UpdateTemplateAsync(updatedTemplate: entity), times: Times.Once);
        templateServiceMock.VerifyNoOtherCalls();
    }

}