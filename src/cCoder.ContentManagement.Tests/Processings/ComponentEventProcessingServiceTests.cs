// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.CMS;
using cCoder.ContentManagement.Services.Foundations.Events;
using cCoder.ContentManagement.Services.Processings;
using FizzWare.NBuilder;
using Moq;


namespace cCoder.Core.Services.Tests.CMS.Processings;

public partial class ComponentEventProcessingServiceTests
{
    private readonly Mock<IComponentEventService> componentEventServiceMock;
    private readonly ComponentEventProcessingService service;
    private const string CurrentUserId = "test-user";

    public ComponentEventProcessingServiceTests()
    {
        componentEventServiceMock = new Mock<IComponentEventService>(behavior: MockBehavior.Strict);
        service = new ComponentEventProcessingService(
            eventService: componentEventServiceMock.Object);
    }

    private static Component CreateRandomComponent() =>
        Builder<Component>.CreateNew()
        .Build();
}