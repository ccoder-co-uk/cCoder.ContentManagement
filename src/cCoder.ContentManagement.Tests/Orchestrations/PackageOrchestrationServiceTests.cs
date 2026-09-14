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

public partial class PackageOrchestrationServiceTests
{
    private readonly Mock<IPackageProcessingService> packageProcessingServiceMock;
    private readonly Mock<IPackageItemProcessingService> packageItemProcessingServiceMock;
    private readonly Mock<IPackageEventProcessingService> packageEventProcessingServiceMock;
    private readonly Mock<IAuthorizationProcessingService> authorizationProcessingServiceMock;
    private readonly PackageOrchestrationService orchestrationService;
    private const string CurrentUserId = "test-user";

    public PackageOrchestrationServiceTests()
    {
        packageProcessingServiceMock = new Mock<IPackageProcessingService>(behavior: MockBehavior.Strict);
        packageItemProcessingServiceMock = new Mock<IPackageItemProcessingService>(behavior: MockBehavior.Strict);
        packageEventProcessingServiceMock = new Mock<IPackageEventProcessingService>(behavior: MockBehavior.Strict);
        authorizationProcessingServiceMock = new(behavior: MockBehavior.Strict);
        authorizationProcessingServiceMock
            .Setup(expression: service => service.GetCurrentUserId())
            .Returns(value: CurrentUserId);

        authorizationProcessingServiceMock
            .Setup(expression: service => service.AuthorizeAuthorizationContext(
                It.IsAny<cCoder.ContentManagement.Models.AuthorizationContext>()));

        orchestrationService = new PackageOrchestrationService(
processingService: packageProcessingServiceMock.Object,
packageItemProcessingService: packageItemProcessingServiceMock.Object,
eventService: packageEventProcessingServiceMock.Object,
authorizationProcessingService: authorizationProcessingServiceMock.Object
        );
    }

    private static Package CreateRandomPackage() =>
        Builder<Package>.CreateNew()
        .Build();
}