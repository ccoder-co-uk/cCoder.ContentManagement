// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.CMS;
using cCoder.ContentManagement.Services.Foundations.Events;
using cCoder.ContentManagement.Services.Processings;
using FizzWare.NBuilder;
using Moq;


namespace cCoder.Core.Services.Tests.CMS.Processings;

public partial class ResourceEventProcessingServiceTests
{
    private readonly Mock<IResourceEventService> resourceEventServiceMock;
    private readonly ResourceEventProcessingService service;
    private const string CurrentUserId = "test-user";

    public ResourceEventProcessingServiceTests()
    {
        resourceEventServiceMock = new Mock<IResourceEventService>(behavior: MockBehavior.Strict);
        service = new ResourceEventProcessingService(
            eventService: resourceEventServiceMock.Object);
    }

    private static Resource CreateRandomResource() =>
        Builder<Resource>.CreateNew()
        .Build();
}