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

public partial class CommonObjectOrchestrationServiceTests
{
    private readonly Mock<ICommonObjectProcessingService> commonObjectProcessingServiceMock;
    private readonly Mock<ICommonObjectEventProcessingService> commonObjectEventProcessingServiceMock;
    private readonly CommonObjectOrchestrationService orchestrationService;

    public CommonObjectOrchestrationServiceTests()
    {
        commonObjectProcessingServiceMock = new Mock<ICommonObjectProcessingService>(behavior: MockBehavior.Strict);
        commonObjectEventProcessingServiceMock = new Mock<ICommonObjectEventProcessingService>(behavior: MockBehavior.Strict);

        orchestrationService = new CommonObjectOrchestrationService(
processingService: commonObjectProcessingServiceMock.Object,
eventService: commonObjectEventProcessingServiceMock.Object
        );
    }

    private static CommonObject CreateRandomCommonObject() =>
        Builder<CommonObject>.CreateNew()
        .Build();
}