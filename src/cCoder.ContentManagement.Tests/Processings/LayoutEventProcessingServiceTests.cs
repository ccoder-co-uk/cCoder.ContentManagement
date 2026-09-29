// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.CMS;
using cCoder.ContentManagement.Services.Foundations.Events;
using cCoder.ContentManagement.Services.Processings;
using FizzWare.NBuilder;
using Moq;


namespace cCoder.Core.Services.Tests.CMS.Processings;

public partial class LayoutEventProcessingServiceTests
{
    private readonly Mock<ILayoutEventService> layoutEventServiceMock;
    private readonly LayoutEventProcessingService service;
    private const string CurrentUserId = "test-user";

    public LayoutEventProcessingServiceTests()
    {
        layoutEventServiceMock = new Mock<ILayoutEventService>(behavior: MockBehavior.Strict);
        service = new LayoutEventProcessingService(
            eventService: layoutEventServiceMock.Object);
    }

    private static Layout CreateRandomLayout() =>
        Builder<Layout>.CreateNew()
        .Build();
}