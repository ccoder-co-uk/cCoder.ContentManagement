// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Threading.Tasks;
using cCoder.Data.Models.CMS;
using cCoder.Eventing.Models;
using FluentAssertions;
using Moq;
using Xunit;
using CmsDataModels = cCoder.Data.Models.CMS;


namespace cCoder.Core.Services.Tests.CMS.Foundations.Events;

public partial class ScriptEventServiceTests
{
    [Fact]
    public async Task ShouldMapAndCallBrokerWhenRaiseScriptUpdateEventAsync()
    {
        // Given
        Script entity = new();
        EventMessage<CmsDataModels.Script> actualMessage = null;

        scriptEventBrokerMock
            .Setup(expression: x => x.RaiseScriptUpdateEventAsync(message: It.IsAny<EventMessage<CmsDataModels.Script>>()))
            .Callback<EventMessage<CmsDataModels.Script>>(action: message => actualMessage = message)
            .Returns(value: ValueTask.CompletedTask);

        // When
        await service.RaiseScriptUpdateEventAsync(
            script: entity,
            userId: CurrentUserId);

        // Then

        actualMessage.Should()
            .NotBeNull();

        actualMessage!.Data.Should()
            .BeEquivalentTo(expectation: entity);

        actualMessage.AuthInfo.Should()
            .NotBeNull();

        actualMessage.AuthInfo.SSOUserId.Should()
            .Be(expected: CurrentUserId);

        scriptEventBrokerMock.Verify(
expression: x => x.RaiseScriptUpdateEventAsync(message: It.IsAny<EventMessage<CmsDataModels.Script>>()),
times: Times.Once
        );

        scriptEventBrokerMock.VerifyNoOtherCalls();
    }

}