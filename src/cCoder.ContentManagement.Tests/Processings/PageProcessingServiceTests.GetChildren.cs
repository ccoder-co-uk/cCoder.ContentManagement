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

namespace cCoder.Core.Services.Tests.CMS.Processings;

public partial class PageProcessingServiceTests
{
    [Fact]
    public void ShouldReturnDirectChildrenWhenGetChildren()
    {
        // Given



        Page parent = CreateRandomPage();
        Page child = CreateRandomPage();
        child.Id = 10;
        child.ParentId = parent.Id;

        pageServiceMock.Setup(expression: x => x.GetAllPage())
            .Returns(value: new[] { parent, child }.AsQueryable());

        // When
        Page[] result = pageProcessingService.GetChildrenPage(pageId: parent.Id)
            .ToArray();

        // Then
        result.Should()
            .ContainSingle();

        result[0].Id.Should()
            .Be(expected: child.Id);

        pageServiceMock.Verify(expression: x => x.GetAllPage(), times: Times.Once);
        pageServiceMock.VerifyNoOtherCalls();
    }

    [Fact]
    public void ShouldReturnEmptyCollectionWhenParentHasNoChildrenForGetChildren()
    {
        // Given



        Page parent = CreateRandomPage();
        Page other = CreateRandomPage();
        other.ParentId = parent.Id + 1;

        pageServiceMock.Setup(expression: x => x.GetAllPage())
            .Returns(value: new[] { parent, other }.AsQueryable());

        // When
        Page[] result = pageProcessingService.GetChildrenPage(pageId: parent.Id)
            .ToArray();

        // Then
        result.Should()
            .BeEmpty();

        pageServiceMock.Verify(expression: x => x.GetAllPage(), times: Times.Once);
        pageServiceMock.VerifyNoOtherCalls();
    }
}