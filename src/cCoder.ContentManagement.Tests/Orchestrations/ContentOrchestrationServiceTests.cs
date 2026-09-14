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

public partial class ContentOrchestrationServiceTests
{
    private readonly Mock<IContentProcessingService> contentProcessingServiceMock;
    private readonly Mock<IPageProcessingService> pageProcessingServiceMock;
    private readonly Mock<IContentEventProcessingService> contentEventProcessingServiceMock;
    private readonly Mock<IAuthorizationProcessingService> authorizationProcessingServiceMock;
    private readonly ContentOrchestrationService orchestrationService;
    private const string CurrentUserId = "test-user";

    public ContentOrchestrationServiceTests()
    {
        contentProcessingServiceMock = new Mock<IContentProcessingService>(behavior: MockBehavior.Strict);
        pageProcessingServiceMock = new Mock<IPageProcessingService>(behavior: MockBehavior.Strict);
        contentEventProcessingServiceMock = new Mock<IContentEventProcessingService>(behavior: MockBehavior.Strict);
        authorizationProcessingServiceMock = new(behavior: MockBehavior.Strict);
        authorizationProcessingServiceMock
            .Setup(expression: service => service.GetCurrentUserId())
            .Returns(value: CurrentUserId);

        authorizationProcessingServiceMock
            .Setup(expression: service => service.AuthorizeAuthorizationContext(
                It.IsAny<cCoder.ContentManagement.Models.AuthorizationContext>()));

        orchestrationService = new ContentOrchestrationService(
processingService: contentProcessingServiceMock.Object,
pageProcessingService: pageProcessingServiceMock.Object,
eventService: contentEventProcessingServiceMock.Object,
authorizationProcessingService: authorizationProcessingServiceMock.Object
        );
    }

    private static Content CreateRandomContent()
    {
        Content content = Builder<Content>.CreateNew().Build();
        content.Page = new Page
        {
            Id = content.PageId,
            AppId = Random.Shared.Next(minValue: 1, maxValue: int.MaxValue)
        };

        return content;
    }
}