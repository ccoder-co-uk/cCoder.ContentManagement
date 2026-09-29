// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models;
using cCoder.ContentManagement.Services.Foundations.Events;
using cCoder.ContentManagement.Services.Processings;
using FizzWare.NBuilder;
using Moq;


namespace cCoder.Core.Services.Tests.CMS.Processings;

public partial class CommonObjectEventProcessingServiceTests
{
    private readonly Mock<ICommonObjectEventService> commonObjectEventServiceMock;
    private readonly CommonObjectEventProcessingService service;
    private const string CurrentUserId = "test-user";

    public CommonObjectEventProcessingServiceTests()
    {
        commonObjectEventServiceMock = new Mock<ICommonObjectEventService>(behavior: MockBehavior.Strict);
        service = new CommonObjectEventProcessingService(
            eventService: commonObjectEventServiceMock.Object);
    }

    private static CommonObject CreateRandomCommonObject() =>
        Builder<CommonObject>.CreateNew()
        .Build();
}