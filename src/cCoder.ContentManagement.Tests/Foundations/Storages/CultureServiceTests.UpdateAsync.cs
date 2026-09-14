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
using System.Security;



using FluentAssertions;
using Moq;
using Xunit;
using CmsDataModels = cCoder.Data.Models.CMS;


namespace cCoder.Core.Services.Tests.CMS.Foundations.Storages;

public partial class CultureServiceTests
{
    [Fact]
    public async Task ShouldDelegateToBrokerWhenUserIsAuthorizedForUpdateAsync()
    {
        // Given
        Culture culture = CreateRandomCulture();

        CmsDataModels.Culture submitted = null;

        cultureBrokerMock
            .Setup(expression: x => x.UpdateCultureAsync(updatedCulture: It.IsAny<CmsDataModels.Culture>()))
            .Callback<CmsDataModels.Culture>(action: candidate => submitted = candidate)
            .ReturnsAsync(valueFunction: (CmsDataModels.Culture value) => value);

        // When
        Culture result = await cultureService.UpdateCultureAsync(updatedCulture: culture);

        // Then

        result.Should()
            .BeSameAs(expected: culture);

        submitted.Should()
            .NotBeNull();

        submitted.Should()
            .NotBeSameAs(unexpected: culture);

        result.Should()
            .NotBeSameAs(unexpected: submitted);

        submitted.Should()
            .BeEquivalentTo(expectation: culture);

        result.Should()
            .BeEquivalentTo(expectation: culture);

        cultureBrokerMock.Verify(expression: x => x.UpdateCultureAsync(updatedCulture: It.IsAny<CmsDataModels.Culture>()), times: Times.Once);
        cultureBrokerMock.VerifyNoOtherCalls();
    }

}