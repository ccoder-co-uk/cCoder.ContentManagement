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

public partial class CultureProcessingServiceTests
{
    [Fact]
    public async Task ShouldDelegateToFoundationServiceWhenUpdateAsync()
    {
        // Given
        Culture entity = CreateRandomCulture();

        cultureServiceMock.Setup(expression: x => x.UpdateCultureAsync(updatedCulture: entity))
            .ReturnsAsync(value: entity);

        // When
        Culture result = await cultureProcessingService.UpdateCultureAsync(updatedCulture: entity);

        // Then

        result.Should()
            .BeSameAs(expected: entity);

        cultureServiceMock.Verify(expression: x => x.UpdateCultureAsync(updatedCulture: entity), times: Times.Once);
        cultureServiceMock.VerifyNoOtherCalls();
    }

}