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
using LocalPageRole = cCoder.Data.Models.Security.PageRole;


namespace cCoder.Core.Services.Tests.CMS.Processings;

public partial class PageRoleProcessingServiceTests
{
    [Fact]
    public void ShouldDelegateToFoundationServiceWhenGetAll()
    {
        // Given
        LocalPageRole[] links =
        [
            new() { PageId = Random.Shared.Next(minValue: 1, maxValue: 1000), RoleId = Guid.NewGuid() },
        ];

        IQueryable<LocalPageRole> queryableLinks = links.AsQueryable();

        pageRoleServiceMock.Setup(expression: x => x.GetAllPageRole())
            .Returns(value: queryableLinks);

        // When
        IQueryable<LocalPageRole> result = pageRoleProcessingService.GetAllPageRole();

        // Then

        result.Should()
            .BeSameAs(expected: queryableLinks);

        pageRoleServiceMock.Verify(expression: x => x.GetAllPageRole(), times: Times.Once);
        pageRoleServiceMock.VerifyNoOtherCalls();
        roleBrokerMock.VerifyNoOtherCalls();
        pageBrokerMock.VerifyNoOtherCalls();
    }

}