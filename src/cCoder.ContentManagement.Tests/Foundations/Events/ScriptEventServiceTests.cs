// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Brokers.Events;
using Moq;


namespace cCoder.Core.Services.Tests.CMS.Foundations.Events;

public partial class ScriptEventServiceTests
{
    private readonly Mock<IScriptEventBroker> scriptEventBrokerMock;
    private readonly cCoder.ContentManagement.Services.Foundations.Events.ScriptEventService service;
    private const string CurrentUserId = "test-user";

    public ScriptEventServiceTests()
    {
        scriptEventBrokerMock = new Mock<IScriptEventBroker>(behavior: MockBehavior.Strict);
        scriptEventBrokerMock = new(behavior: MockBehavior.Strict);

        service = new cCoder.ContentManagement.Services.Foundations.Events.ScriptEventService(
scriptEventBroker: scriptEventBrokerMock.Object
        );
    }
}