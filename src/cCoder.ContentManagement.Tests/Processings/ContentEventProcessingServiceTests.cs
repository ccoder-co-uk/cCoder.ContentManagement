// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.CMS;
using cCoder.ContentManagement.Services.Foundations.Events;
using cCoder.ContentManagement.Services.Processings;
using FizzWare.NBuilder;
using Moq;


namespace cCoder.Core.Services.Tests.CMS.Processings;

public partial class ContentEventProcessingServiceTests
{
    private readonly Mock<IContentEventService> contentEventServiceMock;
    private readonly ContentEventProcessingService service;
    private const string CurrentUserId = "test-user";

    public ContentEventProcessingServiceTests()
    {
        contentEventServiceMock = new Mock<IContentEventService>(behavior: MockBehavior.Strict);
        service = new ContentEventProcessingService(
            eventService: contentEventServiceMock.Object);
    }

    private static Content CreateRandomContent() =>
        Builder<Content>.CreateNew()
        .Build();
}