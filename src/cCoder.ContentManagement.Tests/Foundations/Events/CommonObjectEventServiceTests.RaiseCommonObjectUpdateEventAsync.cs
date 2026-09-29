// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Threading.Tasks;
using cCoder.Data.Models;
using cCoder.Eventing.Models;
using FluentAssertions;
using Moq;
using Xunit;

namespace cCoder.Core.Services.Tests.CMS.Foundations.Events;

public partial class CommonObjectEventServiceTests
{
    [Fact]
    public async Task ShouldMapAndCallBrokerWhenRaiseCommonObjectUpdateEventAsync()
    {
        // Given
        CommonObject entity = new();
        EventMessage<CommonObject> actualMessage = null;

        commonObjectEventBrokerMock
            .Setup(expression: x => x.RaiseCommonObjectUpdateEventAsync(message: It.IsAny<EventMessage<CommonObject>>()))
            .Callback<EventMessage<CommonObject>>(action: message => actualMessage = message)
            .Returns(value: ValueTask.CompletedTask);

        // When
        await service.RaiseCommonObjectUpdateEventAsync(
            commonObject: entity,
            userId: CurrentUserId);

        // Then

        actualMessage.Should()
            .NotBeNull();

        actualMessage!.Data.Should()
            .BeSameAs(expected: entity);

        actualMessage.AuthInfo.Should()
            .NotBeNull();

        actualMessage.AuthInfo.SSOUserId.Should()
            .Be(expected: CurrentUserId);

        commonObjectEventBrokerMock.Verify(
expression: x => x.RaiseCommonObjectUpdateEventAsync(message: It.IsAny<EventMessage<CommonObject>>()),
times: Times.Once
        );

        commonObjectEventBrokerMock.VerifyNoOtherCalls();
    }

}