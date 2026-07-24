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
using FluentAssertions;
using Moq;
using Xunit;


namespace cCoder.Core.Services.Tests.CMS.Processings;

public partial class ComponentProcessingServiceTests
{
    [Fact]
    public async Task ShouldDelegateToFoundationServiceWhenUpdateAsync()
    {
        // Given
        Component entity = CreateRandomComponent();

        componentServiceMock.Setup(expression: x => x.UpdateComponentAsync(updatedComponent: entity))
            .ReturnsAsync(value: entity);

        // When
        Component result = await componentProcessingService.UpdateComponentAsync(updatedComponent: entity);

        // Then

        result.Should()
            .BeSameAs(expected: entity);

        componentServiceMock.Verify(expression: x => x.UpdateComponentAsync(updatedComponent: entity), times: Times.Once);
        componentServiceMock.VerifyNoOtherCalls();
    }

}