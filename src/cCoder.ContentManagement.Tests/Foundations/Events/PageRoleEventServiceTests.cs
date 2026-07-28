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
using cCoder.ContentManagement.Brokers.Events;
using Moq;
using ICoreAuthInfo = cCoder.Data.ICoreAuthInfo;


namespace cCoder.Core.Services.Tests.CMS.Foundations.Events;

public partial class PageRoleEventServiceTests
{
    private readonly Mock<IPageRoleEventBroker> pageRoleEventBrokerMock;
    private readonly cCoder.ContentManagement.Services.Foundations.Events.PageRoleEventService service;
    private const string CurrentUserId = "test-user";

    public PageRoleEventServiceTests()
    {
        pageRoleEventBrokerMock = new Mock<IPageRoleEventBroker>(behavior: MockBehavior.Strict);
        pageRoleEventBrokerMock = new(behavior: MockBehavior.Strict);

        pageRoleEventBrokerMock.Setup(expression: x => x.GetCurrentUserId())
            .Returns(value: CurrentUserId);

        service = new cCoder.ContentManagement.Services.Foundations.Events.PageRoleEventService(
pageRoleEventBroker: pageRoleEventBrokerMock.Object
        );
    }
}