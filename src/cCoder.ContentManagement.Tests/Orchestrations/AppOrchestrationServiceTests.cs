// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.CMS;
using cCoder.ContentManagement.Services.Orchestrations;
using cCoder.ContentManagement.Services.Processings;
using FizzWare.NBuilder;
using Moq;


namespace cCoder.Core.Services.Tests.CMS.Orchestrations;

public partial class AppOrchestrationServiceTests
{
    private readonly Mock<IAppProcessingService> appProcessingServiceMock;
    private readonly Mock<IAppEventProcessingService> appEventProcessingServiceMock;
    private readonly Mock<IAuthorizationProcessingService> authorizationProcessingServiceMock;
    private readonly AppOrchestrationService orchestrationService;
    private const string CurrentUserId = "test-user";

    public AppOrchestrationServiceTests()
    {
        appProcessingServiceMock = new Mock<IAppProcessingService>(behavior: MockBehavior.Strict);
        appEventProcessingServiceMock = new Mock<IAppEventProcessingService>(behavior: MockBehavior.Strict);
        authorizationProcessingServiceMock = new Mock<IAuthorizationProcessingService>(behavior: MockBehavior.Strict);
        authorizationProcessingServiceMock
            .Setup(expression: service => service.GetCurrentUserId())
            .Returns(value: CurrentUserId);

        orchestrationService = new AppOrchestrationService(
            processingService: appProcessingServiceMock.Object,
            authorizationProcessingService: authorizationProcessingServiceMock.Object,
            eventProcessingService: appEventProcessingServiceMock.Object);
    }

    private static App CreateRandomApp() =>
        Builder<App>.CreateNew()
        .Build();
}