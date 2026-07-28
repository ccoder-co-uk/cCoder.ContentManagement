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

public partial class AppCultureServiceTests
{
    [Fact]
    public void ShouldDelegateToBrokerWhenGetAll()
    {
        // Given
        AppCulture[] expectedItems = [CreateRandomAppCulture()];

        IQueryable<CmsDataModels.AppCulture> appCultures = expectedItems
            .Select(selector: item => item)
            .AsQueryable();

        appCultureBrokerMock.Setup(expression: x => x.GetAllAppCultures(ignoreFilters: false))
            .Returns(value: appCultures);

        // When
        IQueryable<AppCulture> result = appCultureService.GetAllAppCulture();

        // Then

        result.Should()
            .BeEquivalentTo(expectation: expectedItems);

        appCultureBrokerMock.Verify(expression: x => x.GetAllAppCultures(ignoreFilters: false), times: Times.Once);
        appCultureBrokerMock.VerifyNoOtherCalls();
        authorizationBrokerMock.VerifyNoOtherCalls();
    }

}