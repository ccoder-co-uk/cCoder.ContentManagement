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

public partial class AppOrchestrationServiceTests
{
    private readonly Mock<IAppProcessingService> appProcessingServiceMock;
    private readonly Mock<IAppEventProcessingService> appEventProcessingServiceMock;
    private readonly Mock<IAuthorizationProcessingService> authorizationProcessingServiceMock;
    private readonly AppOrchestrationService orchestrationService;

    public AppOrchestrationServiceTests()
    {
        appProcessingServiceMock = new Mock<IAppProcessingService>(behavior: MockBehavior.Strict);
        appEventProcessingServiceMock = new Mock<IAppEventProcessingService>(behavior: MockBehavior.Strict);
        authorizationProcessingServiceMock = new Mock<IAuthorizationProcessingService>(behavior: MockBehavior.Strict);

        orchestrationService = new AppOrchestrationService(
processingService: appProcessingServiceMock.Object,
eventService: appEventProcessingServiceMock.Object,
authorizationProcessingService: authorizationProcessingServiceMock.Object
        );
    }

    private static App CreateRandomApp() =>
        Builder<App>.CreateNew()
        .Build();
}