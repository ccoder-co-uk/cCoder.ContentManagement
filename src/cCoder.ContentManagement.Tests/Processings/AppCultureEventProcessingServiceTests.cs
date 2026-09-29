// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.CMS;
using cCoder.ContentManagement.Services.Foundations.Events;
using cCoder.ContentManagement.Services.Processings;
using FizzWare.NBuilder;
using Moq;


namespace cCoder.Core.Services.Tests.CMS.Processings;

public partial class AppCultureEventProcessingServiceTests
{
    private readonly Mock<IAppCultureEventService> appCultureEventServiceMock;
    private readonly AppCultureEventProcessingService service;
    private const string CurrentUserId = "test-user";

    public AppCultureEventProcessingServiceTests()
    {
        appCultureEventServiceMock = new Mock<IAppCultureEventService>(behavior: MockBehavior.Strict);
        service = new AppCultureEventProcessingService(
            eventService: appCultureEventServiceMock.Object);
    }

    private static AppCulture CreateRandomAppCulture() =>
        Builder<AppCulture>.CreateNew()
        .Build();
}