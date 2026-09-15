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
using DataPageInfo = cCoder.Data.Models.CMS.PageInfo;


namespace cCoder.Core.Services.Tests.CMS.Foundations.Storages;

public partial class PageInfoServiceTests
{
    [Fact]
    public async Task ShouldDelegateToBrokerWhenUserIsAuthorizedForAddAsync()
    {
        // Given
        PageInfo pageInfo = CreateRandomPageInfo(id: 0);

        DataPageInfo submitted = null;

        pageInfoBrokerMock
            .Setup(expression: x =>
                x.AddPageInfoAsync(
newPageInfo: It.IsAny<DataPageInfo>()
                )
            )
            .Callback<DataPageInfo>(action: candidate => submitted = candidate)
            .ReturnsAsync(valueFunction: (DataPageInfo value) => value);

        // When
        PageInfo result = await pageInfoService.AddPageInfoAsync(newPageInfo: pageInfo);

        // Then

        result.Should()
            .BeSameAs(expected: pageInfo);

        submitted.Should()
            .NotBeNull();

        submitted.Should()
            .NotBeSameAs(unexpected: pageInfo);

        result.Should()
            .NotBeSameAs(unexpected: submitted);

        submitted
            .Should()
            .BeEquivalentTo(expectation: pageInfo, config: options => options.Excluding(expression: candidate => candidate.Id));

        result
            .Should()
            .BeEquivalentTo(expectation: pageInfo, config: options => options.Excluding(expression: candidate => candidate.Id));

        pageInfoBrokerMock.Verify(
expression: x =>
                x.AddPageInfoAsync(
newPageInfo: It.Is<DataPageInfo>(match: candidate => candidate.Id == pageInfo.Id)
                ),
times: Times.Once
        );

        pageInfoBrokerMock.VerifyNoOtherCalls();
    }

}