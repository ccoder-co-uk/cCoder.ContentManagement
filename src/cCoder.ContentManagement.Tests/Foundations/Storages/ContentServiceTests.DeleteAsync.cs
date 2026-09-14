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

public partial class ContentServiceTests
{
    [Fact]
    public async Task ShouldDelegateToBrokerWhenDeleteAsync()
    {
        // Given
        Content content = CreateRandomContent(id: 9);

        contentBrokerMock.Setup(expression: x => x.GetAllContents())
            .Returns(value: new[] { content }.AsQueryable());

        contentBrokerMock
            .Setup(
expression: x =>
                    x.DeleteContentAsync(
deletedContent: It.Is<CmsDataModels.Content>(match: item => item.Id == content.Id)
                    )
            )
            .ReturnsAsync(value: 1);

        // When
        await contentService.DeleteAsync(contentId: 9);

        // Then
        contentBrokerMock.Verify(expression: x => x.GetAllContents(), times: Times.Once);

        contentBrokerMock.Verify(
expression: x => x.DeleteContentAsync(deletedContent: It.Is<CmsDataModels.Content>(match: item => item.Id == content.Id)),
times: Times.Once
        );

        contentBrokerMock.VerifyNoOtherCalls();
    }

}
