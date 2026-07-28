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

public partial class LayoutOrchestrationServiceTests
{
    private readonly Mock<ILayoutProcessingService> layoutProcessingServiceMock;
    private readonly Mock<ILayoutEventProcessingService> layoutEventProcessingServiceMock;
    private readonly LayoutOrchestrationService orchestrationService;

    public LayoutOrchestrationServiceTests()
    {
        layoutProcessingServiceMock = new Mock<ILayoutProcessingService>(behavior: MockBehavior.Strict);
        layoutEventProcessingServiceMock = new Mock<ILayoutEventProcessingService>(behavior: MockBehavior.Strict);

        orchestrationService = new LayoutOrchestrationService(
processingService: layoutProcessingServiceMock.Object,
eventService: layoutEventProcessingServiceMock.Object
        );
    }

    private static Layout CreateRandomLayout() =>
        Builder<Layout>.CreateNew()
        .Build();
}