// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.CMS;
using cCoder.ContentManagement.Services.Foundations.Events;
using cCoder.ContentManagement.Services.Processings;
using FizzWare.NBuilder;
using Moq;


namespace cCoder.Core.Services.Tests.CMS.Processings;

public partial class TemplateEventProcessingServiceTests
{
    private readonly Mock<ITemplateEventService> templateEventServiceMock;
    private readonly TemplateEventProcessingService service;
    private const string CurrentUserId = "test-user";

    public TemplateEventProcessingServiceTests()
    {
        templateEventServiceMock = new Mock<ITemplateEventService>(behavior: MockBehavior.Strict);
        service = new TemplateEventProcessingService(
            eventService: templateEventServiceMock.Object);
    }

    private static Template CreateRandomTemplate() =>
        Builder<Template>.CreateNew()
        .Build();
}