// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.CMS;
using cCoder.ContentManagement.Services.Foundations.Events;
using cCoder.ContentManagement.Services.Processings;
using FizzWare.NBuilder;
using Moq;


namespace cCoder.Core.Services.Tests.CMS.Processings;

public partial class AppEventProcessingServiceTests
{
    private readonly Mock<IAppEventService> appEventServiceMock;
    private readonly AppEventProcessingService service;
    private const string CurrentUserId = "test-user";

    public AppEventProcessingServiceTests()
    {
        appEventServiceMock = new Mock<IAppEventService>(behavior: MockBehavior.Strict);
        service = new AppEventProcessingService(
            eventService: appEventServiceMock.Object);
    }

    private static App CreateRandomApp() =>
        Builder<App>.CreateNew()
        .Build();
}