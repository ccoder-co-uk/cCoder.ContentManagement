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
using cCoder.ContentManagement.Services.Orchestrations;
using cCoder.ContentManagement.Services.Processings;
using FizzWare.NBuilder;
using Moq;



namespace cCoder.Core.Services.Tests.CMS.Orchestrations;

public partial class PackageOrchestrationServiceTests
{
    private readonly Mock<IPackageProcessingService> packageProcessingServiceMock;
    private readonly Mock<IPackageItemProcessingService> packageItemProcessingServiceMock;
    private readonly Mock<IPackageEventProcessingService> packageEventProcessingServiceMock;
    private readonly PackageOrchestrationService orchestrationService;

    public PackageOrchestrationServiceTests()
    {
        packageProcessingServiceMock = new Mock<IPackageProcessingService>(behavior: MockBehavior.Strict);
        packageItemProcessingServiceMock = new Mock<IPackageItemProcessingService>(behavior: MockBehavior.Strict);
        packageEventProcessingServiceMock = new Mock<IPackageEventProcessingService>(behavior: MockBehavior.Strict);

        orchestrationService = new PackageOrchestrationService(
processingService: packageProcessingServiceMock.Object,
packageItemProcessingService: packageItemProcessingServiceMock.Object,
eventService: packageEventProcessingServiceMock.Object
        );
    }

    private static Package CreateRandomPackage() =>
        Builder<Package>.CreateNew()
        .Build();
}