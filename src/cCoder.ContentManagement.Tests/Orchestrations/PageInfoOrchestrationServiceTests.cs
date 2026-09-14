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
using cCoder.ContentManagement.Services.Orchestrations;
using cCoder.ContentManagement.Services.Processings;
using FizzWare.NBuilder;
using Moq;

namespace cCoder.Core.Services.Tests.CMS.Orchestrations;

public partial class PageInfoOrchestrationServiceTests
{
    private readonly Mock<IPageInfoProcessingService> pageInfoProcessingServiceMock;
    private readonly Mock<IPageInfoEventProcessingService> pageInfoEventProcessingServiceMock;
    private readonly Mock<IAuthorizationProcessingService> authorizationProcessingServiceMock;
    private readonly PageInfoOrchestrationService orchestrationService;
    private const string CurrentUserId = "test-user";

    public PageInfoOrchestrationServiceTests()
    {
        pageInfoProcessingServiceMock = new Mock<IPageInfoProcessingService>(behavior: MockBehavior.Strict);
        pageInfoEventProcessingServiceMock = new Mock<IPageInfoEventProcessingService>(behavior: MockBehavior.Strict);
        authorizationProcessingServiceMock = new(behavior: MockBehavior.Strict);
        authorizationProcessingServiceMock
            .Setup(expression: service => service.GetCurrentUserId())
            .Returns(value: CurrentUserId);

        orchestrationService = new PageInfoOrchestrationService(
processingService: pageInfoProcessingServiceMock.Object,
eventService: pageInfoEventProcessingServiceMock.Object,
authorizationProcessingService: authorizationProcessingServiceMock.Object
        );
    }

    private static PageInfo CreateRandomPageInfo() =>
        Builder<PageInfo>.CreateNew()
        .Build();
}