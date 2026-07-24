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
using cCoder.ContentManagement.Brokers.Events;
using cCoder.Data;
using Moq;


namespace cCoder.Core.Services.Tests.CMS.Foundations.Events;

public partial class TemplateEventServiceTests
{
    private readonly Mock<ITemplateEventBroker> templateEventBrokerMock;
    private readonly cCoder.ContentManagement.Services.Foundations.Events.TemplateEventService service;
    private const string CurrentUserId = "test-user";

    public TemplateEventServiceTests()
    {
        templateEventBrokerMock = new Mock<ITemplateEventBroker>(behavior: MockBehavior.Strict);
        templateEventBrokerMock = new(behavior: MockBehavior.Strict);

        templateEventBrokerMock.Setup(expression: x => x.GetCurrentUserId())
            .Returns(value: CurrentUserId);

        service = new cCoder.ContentManagement.Services.Foundations.Events.TemplateEventService(
templateEventBroker: templateEventBrokerMock.Object
        );
    }
}