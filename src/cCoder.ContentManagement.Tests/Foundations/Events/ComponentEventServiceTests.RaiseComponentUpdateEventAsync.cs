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

public partial class ComponentEventServiceTests
{
    [Fact]
    public async Task ShouldMapAndCallBrokerWhenRaiseComponentUpdateEventAsync()
    {
        // Given
        Component entity = new();
        EventMessage<CmsDataModels.Component> actualMessage = null;

        componentEventBrokerMock
            .Setup(expression: x => x.RaiseComponentUpdateEventAsync(message: It.IsAny<EventMessage<CmsDataModels.Component>>()))
            .Callback<EventMessage<CmsDataModels.Component>>(action: message => actualMessage = message)
            .Returns(value: ValueTask.CompletedTask);

        // When
        await service.RaiseComponentUpdateEventAsync(
            component: entity,
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

        componentEventBrokerMock.Verify(
expression: x => x.RaiseComponentUpdateEventAsync(message: It.IsAny<EventMessage<CmsDataModels.Component>>()),
times: Times.Once
        );

        componentEventBrokerMock.VerifyNoOtherCalls();
    }

}