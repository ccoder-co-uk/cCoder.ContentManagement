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

public partial class AppCultureOrchestrationServiceTests
{
    private readonly Mock<IAppCultureProcessingService> appCultureProcessingServiceMock;
    private readonly Mock<IAppCultureEventProcessingService> appCultureEventProcessingServiceMock;
    private readonly AppCultureOrchestrationService orchestrationService;

    public AppCultureOrchestrationServiceTests()
    {
        appCultureProcessingServiceMock = new Mock<IAppCultureProcessingService>(behavior: MockBehavior.Strict);
        appCultureEventProcessingServiceMock = new Mock<IAppCultureEventProcessingService>(behavior: MockBehavior.Strict);

        orchestrationService = new AppCultureOrchestrationService(
processingService: appCultureProcessingServiceMock.Object,
eventService: appCultureEventProcessingServiceMock.Object
        );
    }

    private static AppCulture CreateRandomAppCulture() =>
        Builder<AppCulture>.CreateNew()
        .Build();
}