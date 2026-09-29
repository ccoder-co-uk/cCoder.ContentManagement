// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.CMS;
using cCoder.ContentManagement.Services.Foundations.Events;
using cCoder.ContentManagement.Services.Processings;
using FizzWare.NBuilder;
using Moq;


namespace cCoder.Core.Services.Tests.CMS.Processings;

public partial class PageInfoEventProcessingServiceTests
{
    private readonly Mock<IPageInfoEventService> pageInfoEventServiceMock;
    private readonly PageInfoEventProcessingService service;
    private const string CurrentUserId = "test-user";

    public PageInfoEventProcessingServiceTests()
    {
        pageInfoEventServiceMock = new Mock<IPageInfoEventService>(behavior: MockBehavior.Strict);
        service = new PageInfoEventProcessingService(
            eventService: pageInfoEventServiceMock.Object);
    }

    private static PageInfo CreateRandomPageInfo() =>
        Builder<PageInfo>.CreateNew()
        .Build();
}