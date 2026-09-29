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

public partial class LayoutEventServiceTests
{
    [Fact]
    public async Task ShouldMapAndCallBrokerWhenRaiseLayoutUpdateEventAsync()
    {
        // Given
        Layout entity = new();
        EventMessage<CmsDataModels.Layout> actualMessage = null;

        layoutEventBrokerMock
            .Setup(expression: x => x.RaiseLayoutUpdateEventAsync(message: It.IsAny<EventMessage<CmsDataModels.Layout>>()))
            .Callback<EventMessage<CmsDataModels.Layout>>(action: message => actualMessage = message)
            .Returns(value: ValueTask.CompletedTask);

        // When
        await service.RaiseLayoutUpdateEventAsync(
            layout: entity,
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

        layoutEventBrokerMock.Verify(
expression: x => x.RaiseLayoutUpdateEventAsync(message: It.IsAny<EventMessage<CmsDataModels.Layout>>()),
times: Times.Once
        );

        layoutEventBrokerMock.VerifyNoOtherCalls();
    }

}