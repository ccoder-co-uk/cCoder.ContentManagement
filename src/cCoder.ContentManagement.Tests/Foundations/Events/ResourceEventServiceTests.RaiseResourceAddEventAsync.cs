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

public partial class ResourceEventServiceTests
{
    [Fact]
    public async Task ShouldMapAndCallBrokerWhenRaiseResourceAddEventAsync()
    {
        // Given
        Resource entity = new();
        EventMessage<CmsDataModels.Resource> actualMessage = null;

        resourceEventBrokerMock
            .Setup(expression: x => x.RaiseResourceAddEventAsync(message: It.IsAny<EventMessage<CmsDataModels.Resource>>()))
            .Callback<EventMessage<CmsDataModels.Resource>>(action: message => actualMessage = message)
            .Returns(value: ValueTask.CompletedTask);

        // When
        await service.RaiseResourceAddEventAsync(
            resource: entity,
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

        resourceEventBrokerMock.Verify(
expression: x => x.RaiseResourceAddEventAsync(message: It.IsAny<EventMessage<CmsDataModels.Resource>>()),
times: Times.Once
        );

        resourceEventBrokerMock.VerifyNoOtherCalls();
    }

}