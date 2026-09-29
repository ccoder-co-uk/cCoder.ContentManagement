// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.CMS;
using cCoder.ContentManagement.Services.Orchestrations;
using cCoder.ContentManagement.Services.Processings;
using FizzWare.NBuilder;
using Moq;
using cCoder.ContentManagement.Models;


namespace cCoder.Core.Services.Tests.CMS.Orchestrations;

public partial class ComponentOrchestrationServiceTests
{
    private readonly Mock<IComponentProcessingService> componentProcessingServiceMock;
    private readonly Mock<IComponentEventProcessingService> componentEventProcessingServiceMock;
    private readonly Mock<IAuthorizationProcessingService> authorizationProcessingServiceMock;
    private readonly ComponentOrchestrationService orchestrationService;
    private const string CurrentUserId = "test-user";

    public ComponentOrchestrationServiceTests()
    {
        componentProcessingServiceMock = new Mock<IComponentProcessingService>(behavior: MockBehavior.Strict);
        componentEventProcessingServiceMock = new Mock<IComponentEventProcessingService>(behavior: MockBehavior.Strict);
        authorizationProcessingServiceMock = new(behavior: MockBehavior.Strict);
        authorizationProcessingServiceMock
            .Setup(expression: service => service.GetCurrentUserId())
            .Returns(value: CurrentUserId);

        authorizationProcessingServiceMock
            .Setup(expression: service => service.AuthorizeAuthorizationContext(
context:                 It.IsAny<AuthorizationContext>()));

        orchestrationService = new ComponentOrchestrationService(
processingService: componentProcessingServiceMock.Object,
eventService: componentEventProcessingServiceMock.Object,
authorizationProcessingService: authorizationProcessingServiceMock.Object
        );
    }

    private static Component CreateRandomComponent() =>
        Builder<Component>.CreateNew()
        .Build();
}