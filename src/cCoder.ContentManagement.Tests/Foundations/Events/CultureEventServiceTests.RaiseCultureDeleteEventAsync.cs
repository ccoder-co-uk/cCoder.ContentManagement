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

public partial class CultureEventServiceTests
{
    [Fact]
    public async Task ShouldMapAndCallBrokerWhenRaiseCultureDeleteEventAsync()
    {
        // Given
        Culture entity = new();
        EventMessage<CmsDataModels.Culture> actualMessage = null;

        cultureEventBrokerMock
            .Setup(expression: x => x.RaiseCultureDeleteEventAsync(message: It.IsAny<EventMessage<CmsDataModels.Culture>>()))
            .Callback<EventMessage<CmsDataModels.Culture>>(action: message => actualMessage = message)
            .Returns(value: ValueTask.CompletedTask);

        // When
        await service.RaiseCultureDeleteEventAsync(
            culture: entity,
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

        cultureEventBrokerMock.Verify(
expression: x => x.RaiseCultureDeleteEventAsync(message: It.IsAny<EventMessage<CmsDataModels.Culture>>()),
times: Times.Once
        );

        cultureEventBrokerMock.VerifyNoOtherCalls();
    }

}