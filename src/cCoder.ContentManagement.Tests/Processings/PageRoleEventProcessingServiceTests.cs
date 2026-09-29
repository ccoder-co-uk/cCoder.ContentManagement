// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Security;
using cCoder.ContentManagement.Services.Foundations.Events;
using cCoder.ContentManagement.Services.Processings;
using FizzWare.NBuilder;
using Moq;


namespace cCoder.Core.Services.Tests.CMS.Processings;

public partial class PageRoleEventProcessingServiceTests
{
    private readonly Mock<IPageRoleEventService> pageRoleEventServiceMock;
    private readonly PageRoleEventProcessingService service;
    private const string CurrentUserId = "test-user";

    public PageRoleEventProcessingServiceTests()
    {
        pageRoleEventServiceMock = new Mock<IPageRoleEventService>(behavior: MockBehavior.Strict);
        service = new PageRoleEventProcessingService(
            eventService: pageRoleEventServiceMock.Object);
    }

    private static PageRole CreateRandomPageRole() =>
        Builder<PageRole>.CreateNew()
        .Build();
}