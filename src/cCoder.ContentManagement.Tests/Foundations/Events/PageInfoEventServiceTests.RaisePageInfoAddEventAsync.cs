// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Threading.Tasks;
using cCoder.Data.Models.CMS;
using cCoder.Eventing.Models;
using FluentAssertions;
using Moq;
using Xunit;


namespace cCoder.Core.Services.Tests.CMS.Foundations.Events;

public partial class PageInfoEventServiceTests
{
    [Fact]
    public async Task ShouldMapAndCallBrokerWhenRaisePageInfoAddEventAsync()
    {
        // Given
        PageInfo entity = new();
        EventMessage<PageInfo> actualMessage = null;

        pageInfoEventBrokerMock
            .Setup(expression: x => x.RaisePageInfoAddEventAsync(message: It.IsAny<EventMessage<PageInfo>>()))
            .Callback<EventMessage<PageInfo>>(action: message => actualMessage = message)
            .Returns(value: ValueTask.CompletedTask);

        // When
        await service.RaisePageInfoAddEventAsync(
            pageInfo: entity,
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

        pageInfoEventBrokerMock.Verify(
expression: x => x.RaisePageInfoAddEventAsync(message: It.IsAny<EventMessage<PageInfo>>()),
times: Times.Once
        );

        pageInfoEventBrokerMock.VerifyNoOtherCalls();
    }

}