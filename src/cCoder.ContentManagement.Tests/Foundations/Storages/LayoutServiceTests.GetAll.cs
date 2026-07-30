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
using CmsDataModels = cCoder.Data.Models.CMS;


namespace cCoder.Core.Services.Tests.CMS.Foundations.Storages;

public partial class LayoutServiceTests
{
    [Fact]
    public void ShouldDelegateToBrokerWhenGetAll()
    {
        // Given
        Layout[] expectedItems = [CreateRandomLayout()];

        IQueryable<CmsDataModels.Layout> layouts = expectedItems
            .Select(selector: item => item)
            .AsQueryable();

        layoutBrokerMock.Setup(expression: x => x.GetAllLayouts())
            .Returns(value: layouts);

        // When
        IQueryable<Layout> result = layoutService.GetAllLayout();

        // Then

        result.Should()
            .BeEquivalentTo(expectation: expectedItems);

        layoutBrokerMock.Verify(expression: x => x.GetAllLayouts(), times: Times.Once);
        layoutBrokerMock.VerifyNoOtherCalls();
        authorizationManagerMock.VerifyNoOtherCalls();
    }

}