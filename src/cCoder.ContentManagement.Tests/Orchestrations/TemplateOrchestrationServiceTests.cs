// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.CMS;
using cCoder.ContentManagement.Services.Orchestrations;
using cCoder.ContentManagement.Services.Processings;
using FizzWare.NBuilder;
using Moq;


namespace cCoder.Core.Services.Tests.CMS.Orchestrations;

public partial class TemplateOrchestrationServiceTests
{
    private readonly Mock<ITemplateProcessingService> templateProcessingServiceMock;
    private readonly Mock<ITemplateEventProcessingService> templateEventProcessingServiceMock;
    private readonly Mock<IAuthorizationProcessingService> authorizationProcessingServiceMock;
    private readonly TemplateOrchestrationService orchestrationService;
    private const string CurrentUserId = "test-user";

    public TemplateOrchestrationServiceTests()
    {
        templateProcessingServiceMock = new Mock<ITemplateProcessingService>(behavior: MockBehavior.Strict);
        templateEventProcessingServiceMock = new Mock<ITemplateEventProcessingService>(behavior: MockBehavior.Strict);
        authorizationProcessingServiceMock = new(behavior: MockBehavior.Strict);
        authorizationProcessingServiceMock
            .Setup(expression: service => service.GetCurrentUserId())
            .Returns(value: CurrentUserId);

        authorizationProcessingServiceMock
            .Setup(expression: service => service.AuthorizeAuthorizationContext(
                context: It.IsAny<cCoder.ContentManagement.Models.AuthorizationContext>()));

        orchestrationService = new TemplateOrchestrationService(
processingService: templateProcessingServiceMock.Object,
eventService: templateEventProcessingServiceMock.Object,
authorizationProcessingService: authorizationProcessingServiceMock.Object
        );
    }

    private static Template CreateRandomTemplate() =>
        Builder<Template>.CreateNew()
        .Build();
}