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

public partial class ResourceOrchestrationServiceTests
{
    private readonly Mock<IResourceProcessingService> resourceProcessingServiceMock;
    private readonly Mock<IResourceEventProcessingService> resourceEventProcessingServiceMock;
    private readonly Mock<IAuthorizationProcessingService> authorizationProcessingServiceMock;
    private readonly ResourceOrchestrationService orchestrationService;
    private const string CurrentUserId = "test-user";

    public ResourceOrchestrationServiceTests()
    {
        resourceProcessingServiceMock = new Mock<IResourceProcessingService>(behavior: MockBehavior.Strict);
        resourceEventProcessingServiceMock = new Mock<IResourceEventProcessingService>(behavior: MockBehavior.Strict);
        authorizationProcessingServiceMock = new(behavior: MockBehavior.Strict);
        authorizationProcessingServiceMock
            .Setup(expression: service => service.GetCurrentUserId())
            .Returns(value: CurrentUserId);

        authorizationProcessingServiceMock
            .Setup(expression: service => service.AuthorizeAuthorizationContext(
context:                 It.IsAny<cCoder.ContentManagement.Models.AuthorizationContext>()));

        orchestrationService = new ResourceOrchestrationService(
processingService: resourceProcessingServiceMock.Object,
eventService: resourceEventProcessingServiceMock.Object,
authorizationProcessingService: authorizationProcessingServiceMock.Object
        );
    }

    private static Resource CreateRandomResource() =>
        Builder<Resource>.CreateNew()
        .Build();
}