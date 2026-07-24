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


namespace cCoder.Core.Services.Tests.CMS.Orchestrations;

public partial class AppOrchestrationServiceTests
{
    [Fact]
    public void ShouldReturnProcessingResultsWhenGetAppUsers()
    {
        const int appId = 1;
        IQueryable<User> users = new[] { new User { Id = "user-id" } }.AsQueryable();

        appProcessingServiceMock.Setup(expression: x => x.GetAppUsers(appId: appId))
            .Returns(value: users);

        var result = orchestrationService.GetAppUsers(appId: appId)
            .ToArray();

        result.Select(selector: item => item.Id)
            .Should()
            .Equal(expected: users.Select(selector: item => item.Id));

        appProcessingServiceMock.Verify(expression: x => x.GetAppUsers(appId: appId), times: Times.Once);
        appProcessingServiceMock.VerifyNoOtherCalls();
        appEventProcessingServiceMock.VerifyNoOtherCalls();
    }

}