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
using cCoder.ContentManagement.Rendering.Services.Processings;
using FizzWare.NBuilder;
using Moq;


namespace cCoder.Core.Services.Tests.CMS.Orchestrations;

public partial class CommonObjectOrchestrationServiceTests
{
    private readonly Mock<ICommonObjectProcessingService> commonObjectProcessingServiceMock;
    private readonly Mock<ICommonObjectLatestCacheProcessingService> cacheProcessingServiceMock;
    private readonly Mock<IAuthorizationProcessingService> authorizationProcessingServiceMock;
    private readonly CommonObjectOrchestrationService orchestrationService;
    private const string CurrentUserId = "test-user";

    public CommonObjectOrchestrationServiceTests()
    {
        commonObjectProcessingServiceMock = new Mock<ICommonObjectProcessingService>(behavior: MockBehavior.Strict);
        cacheProcessingServiceMock = new Mock<ICommonObjectLatestCacheProcessingService>(behavior: MockBehavior.Strict);
        authorizationProcessingServiceMock = new(behavior: MockBehavior.Strict);
        authorizationProcessingServiceMock
            .Setup(expression: service => service.GetCurrentUserId())
            .Returns(value: CurrentUserId);

        orchestrationService = new CommonObjectOrchestrationService(
processingService: commonObjectProcessingServiceMock.Object,
latestCacheProcessingService: cacheProcessingServiceMock.Object,
authorizationProcessingService: authorizationProcessingServiceMock.Object
        );
    }

    private static CommonObject CreateRandomCommonObject() =>
        Builder<CommonObject>.CreateNew()
        .Build();

    private void SetupAuthorization(string privilege) =>
        authorizationProcessingServiceMock
            .Setup(service => service.AuthorizeAuthorizationContext(
                It.Is<cCoder.ContentManagement.Models.AuthorizationContext>(context =>
                    context.Request.AppId == null
                    && context.Request.Privilege == privilege)));
}