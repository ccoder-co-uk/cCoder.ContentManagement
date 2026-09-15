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
using System.Linq.Expressions;


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
        // Given
        commonObjectProcessingServiceMock = new Mock<ICommonObjectProcessingService>(behavior: MockBehavior.Strict);
        cacheProcessingServiceMock = new Mock<ICommonObjectLatestCacheProcessingService>(behavior: MockBehavior.Strict);
        authorizationProcessingServiceMock = new(behavior: MockBehavior.Strict);
        authorizationProcessingServiceMock
            .Setup(expression: service => service.GetCurrentUserId())
            .Returns(value: CurrentUserId);

        // When
        orchestrationService = new CommonObjectOrchestrationService(
            processingService: commonObjectProcessingServiceMock.Object,
            latestCacheProcessingService: cacheProcessingServiceMock.Object,
            authorizationProcessingService: authorizationProcessingServiceMock.Object);

        // Then
    }

    private static CommonObject CreateRandomCommonObject()
    {
        // Given

        // When
        CommonObject commonObject = Builder<CommonObject>
            .CreateNew()
            .Build();

        // Then
        return commonObject;
    }

    private void ShouldSetupAuthorization(string privilege)
    {
        // Given
        Expression<Action<IAuthorizationProcessingService>> authorizationExpression =
            service => service.AuthorizeAuthorizationContext(
                context: It.Is<cCoder.ContentManagement.Models.AuthorizationContext>(match: context =>
                    context.Request.AppId == null
                    && context.Request.Privilege == privilege));

        // When
        authorizationProcessingServiceMock
            .Setup(expression: authorizationExpression);

        // Then
    }
}