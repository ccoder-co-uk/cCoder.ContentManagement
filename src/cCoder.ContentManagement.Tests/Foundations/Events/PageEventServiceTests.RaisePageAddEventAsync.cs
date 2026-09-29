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

public partial class PageEventServiceTests
{
    [Fact]
    public async Task ShouldMapAndCallBrokerWhenRaisePageAddEventAsync()
    {
        // Given
        Page entity = new();
        EventMessage<CmsDataModels.Page> actualMessage = null;

        pageEventBrokerMock
            .Setup(expression: x => x.RaisePageAddEventAsync(message: It.IsAny<EventMessage<CmsDataModels.Page>>()))
            .Callback<EventMessage<CmsDataModels.Page>>(action: message => actualMessage = message)
            .Returns(value: ValueTask.CompletedTask);

        // When
        await service.RaisePageAddEventAsync(
            page: entity,
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

        pageEventBrokerMock.Verify(
expression: x => x.RaisePageAddEventAsync(message: It.IsAny<EventMessage<CmsDataModels.Page>>()),
times: Times.Once
        );

        pageEventBrokerMock.VerifyNoOtherCalls();
    }

}