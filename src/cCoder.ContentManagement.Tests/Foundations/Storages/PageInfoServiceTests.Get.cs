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


namespace cCoder.Core.Services.Tests.CMS.Foundations.Storages;

public partial class PageInfoServiceTests
{
    [Fact]
    public void ShouldDelegateToBrokerWhenGet()
    {
        // Given
        PageInfo pageInfo = CreateRandomPageInfo(id: 7);

        pageInfoBrokerMock.Setup(expression: x => x.GetAllPageInfo(ignoreFilters: false))
            .Returns(value: new[] { ToDataPageInfo(pageInfo: pageInfo) }.AsQueryable());

        // When
        PageInfo result = pageInfoService.GetPageInfo(pageInfoId: 7);

        // Then

        result.Should()
            .BeEquivalentTo(expectation: pageInfo);

        pageInfoBrokerMock.Verify(expression: x => x.GetAllPageInfo(ignoreFilters: false), times: Times.Once);
        pageInfoBrokerMock.VerifyNoOtherCalls();
        pageBrokerMock.VerifyNoOtherCalls();
        authorizationBrokerMock.VerifyNoOtherCalls();
    }

}