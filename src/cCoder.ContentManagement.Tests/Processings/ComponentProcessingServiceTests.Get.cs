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

public partial class ComponentProcessingServiceTests
{
    [Fact]
    public void ShouldDelegateToFoundationServiceWhenGet()
    {
        // Given
        Component entity = CreateRandomComponent();
        var id = entity.Id;

        componentServiceMock.Setup(expression: x => x.GetComponent(componentId: id))
            .Returns(value: entity);

        // When
        Component result = componentProcessingService.GetComponent(componentId: id);

        // Then

        result.Should()
            .BeSameAs(expected: entity);

        componentServiceMock.Verify(expression: x => x.GetComponent(componentId: id), times: Times.Once);
        componentServiceMock.VerifyNoOtherCalls();
    }

}