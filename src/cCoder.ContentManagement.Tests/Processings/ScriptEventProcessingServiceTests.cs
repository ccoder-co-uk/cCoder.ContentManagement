// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.CMS;
using cCoder.ContentManagement.Services.Foundations.Events;
using cCoder.ContentManagement.Services.Processings;
using FizzWare.NBuilder;
using Moq;


namespace cCoder.Core.Services.Tests.CMS.Processings;

public partial class ScriptEventProcessingServiceTests
{
    private readonly Mock<IScriptEventService> scriptEventServiceMock;
    private readonly ScriptEventProcessingService service;
    private const string CurrentUserId = "test-user";

    public ScriptEventProcessingServiceTests()
    {
        scriptEventServiceMock = new Mock<IScriptEventService>(behavior: MockBehavior.Strict);
        service = new ScriptEventProcessingService(
            eventService: scriptEventServiceMock.Object);
    }

    private static Script CreateRandomScript() =>
        Builder<Script>.CreateNew()
        .Build();
}