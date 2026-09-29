// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Threading.Tasks;
using cCoder.Data.Models.Security;
using cCoder.Eventing.Models;
using FluentAssertions;
using Moq;
using Xunit;


namespace cCoder.Core.Services.Tests.CMS.Foundations.Events;

public partial class PageRoleEventServiceTests
{
    [Fact]
    public async Task ShouldMapAndCallBrokerWhenRaisePageRoleAddEventAsync()
    {
        // Given
        PageRole entity = new();
        EventMessage<PageRole> actualMessage = null;

        pageRoleEventBrokerMock
            .Setup(expression: x => x.RaisePageRoleAddEventAsync(message: It.IsAny<EventMessage<PageRole>>()))
            .Callback<EventMessage<PageRole>>(action: message => actualMessage = message)
            .Returns(value: ValueTask.CompletedTask);

        // When
        await service.RaisePageRoleAddEventAsync(
            pageRole: entity,
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

        pageRoleEventBrokerMock.Verify(
expression: x => x.RaisePageRoleAddEventAsync(message: It.IsAny<EventMessage<PageRole>>()),
times: Times.Once
        );

        pageRoleEventBrokerMock.VerifyNoOtherCalls();
    }

}