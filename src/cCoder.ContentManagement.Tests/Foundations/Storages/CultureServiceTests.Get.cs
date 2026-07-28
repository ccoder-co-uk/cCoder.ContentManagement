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


namespace cCoder.Core.Services.Tests.CMS.Foundations.Storages;

public partial class CultureServiceTests
{
    [Fact]
    public void ShouldDelegateToBrokerWhenGet()
    {
        // Given
        Culture culture = CreateRandomCulture(id: "en-GB");

        cultureBrokerMock.Setup(expression: x => x.GetAllCultures(ignoreFilters: false))
            .Returns(value: new[] { culture }.AsQueryable());

        // When
        Culture result = cultureService.GetCulture(cultureId: "en-GB");

        // Then

        result.Should()
            .BeEquivalentTo(expectation: culture);

        cultureBrokerMock.Verify(expression: x => x.GetAllCultures(ignoreFilters: false), times: Times.Once);
        cultureBrokerMock.VerifyNoOtherCalls();
        appCultureBrokerMock.VerifyNoOtherCalls();
        authorizationBrokerMock.VerifyNoOtherCalls();
    }

}