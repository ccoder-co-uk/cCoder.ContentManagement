// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.CMS;
using cCoder.ContentManagement.Services.Foundations.Events;
using cCoder.ContentManagement.Services.Processings;
using FizzWare.NBuilder;
using Moq;


namespace cCoder.Core.Services.Tests.CMS.Processings;

public partial class PageEventProcessingServiceTests
{
    private readonly Mock<IPageEventService> pageEventServiceMock;
    private readonly PageEventProcessingService service;
    private const string CurrentUserId = "test-user";

    public PageEventProcessingServiceTests()
    {
        pageEventServiceMock = new Mock<IPageEventService>(behavior: MockBehavior.Strict);
        service = new PageEventProcessingService(
            eventService: pageEventServiceMock.Object);
    }

    private static Page CreateRandomPage() =>
        Builder<Page>.CreateNew()
        .Build();
}