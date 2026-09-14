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

public partial class PageOrchestrationServiceTests
{
    private readonly Mock<IPageProcessingService> pageProcessingServiceMock;
    private readonly Mock<IPageEventProcessingService> pageEventProcessingServiceMock;
    private readonly Mock<ILayoutProcessingService> layoutProcessingServiceMock;
    private readonly Mock<IAuthorizationProcessingService> authorizationProcessingServiceMock;
    private readonly PageOrchestrationService orchestrationService;
    private const string CurrentUserId = "test-user";

    public PageOrchestrationServiceTests()
    {
        pageProcessingServiceMock = new Mock<IPageProcessingService>(behavior: MockBehavior.Strict);
        pageEventProcessingServiceMock = new Mock<IPageEventProcessingService>(behavior: MockBehavior.Strict);
        layoutProcessingServiceMock = new Mock<ILayoutProcessingService>(behavior: MockBehavior.Strict);
        authorizationProcessingServiceMock = new(behavior: MockBehavior.Strict);
        authorizationProcessingServiceMock
            .Setup(expression: service => service.GetCurrentUserId())
            .Returns(value: CurrentUserId);

        orchestrationService = new PageOrchestrationService(
processingService: pageProcessingServiceMock.Object,
eventService: pageEventProcessingServiceMock.Object,
layoutProcessingService: layoutProcessingServiceMock.Object,
authorizationProcessingService: authorizationProcessingServiceMock.Object
        );
    }

    private static Page CreateRandomPage() =>
        Builder<Page>.CreateNew()
        .Build();

    private static Layout CreateLayoutFor(Page page) =>
        Builder<Layout>
            .CreateNew()
        .With(func: layout => layout.AppId = page.AppId)
        .With(func: layout => layout.Name = page.Layout)
        .Build();
}