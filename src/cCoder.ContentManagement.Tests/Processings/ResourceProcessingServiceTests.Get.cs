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

public partial class ResourceProcessingServiceTests
{
    [Fact]
    public void ShouldDelegateToFoundationServiceWhenGet()
    {
        // Given
        Resource resource = CreateRandomResource();

        resourceServiceMock.Setup(expression: x => x.GetResource(resourceId: resource.Id))
            .Returns(value: resource);

        // When
        Resource result = resourceProcessingService.GetResource(resourceId: resource.Id);

        // Then

        result.Should()
            .BeSameAs(expected: resource);

        resourceServiceMock.Verify(expression: x => x.GetResource(resourceId: resource.Id), times: Times.Once);
        resourceServiceMock.VerifyNoOtherCalls();
    }

}