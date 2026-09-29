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

public partial class AppEventServiceTests
{
    [Fact]
    public async Task ShouldMapAndCallBrokerWhenRaiseAppUpdateEventAsync()
    {
        // Given
        App entity = new();
        EventMessage<CmsDataModels.App> actualMessage = null;

        appEventBrokerMock
            .Setup(expression: x => x.RaiseAppUpdateEventAsync(message: It.IsAny<EventMessage<CmsDataModels.App>>()))
            .Callback<EventMessage<CmsDataModels.App>>(action: message => actualMessage = message)
            .Returns(value: ValueTask.CompletedTask);

        // When
        await service.RaiseAppUpdateEventAsync(
            app: entity,
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

        appEventBrokerMock.Verify(
expression: x => x.RaiseAppUpdateEventAsync(message: It.IsAny<EventMessage<CmsDataModels.App>>()),
times: Times.Once
        );

        appEventBrokerMock.VerifyNoOtherCalls();
    }

}