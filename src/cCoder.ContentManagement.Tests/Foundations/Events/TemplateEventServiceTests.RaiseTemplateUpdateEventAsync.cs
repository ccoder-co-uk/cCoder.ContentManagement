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

public partial class TemplateEventServiceTests
{
    [Fact]
    public async Task ShouldMapAndCallBrokerWhenRaiseTemplateUpdateEventAsync()
    {
        // Given
        Template entity = new();
        EventMessage<CmsDataModels.Template> actualMessage = null;

        templateEventBrokerMock
            .Setup(expression: x => x.RaiseTemplateUpdateEventAsync(message: It.IsAny<EventMessage<CmsDataModels.Template>>()))
            .Callback<EventMessage<CmsDataModels.Template>>(action: message => actualMessage = message)
            .Returns(value: ValueTask.CompletedTask);

        // When
        await service.RaiseTemplateUpdateEventAsync(
            template: entity,
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

        templateEventBrokerMock.Verify(
expression: x => x.RaiseTemplateUpdateEventAsync(message: It.IsAny<EventMessage<CmsDataModels.Template>>()),
times: Times.Once
        );

        templateEventBrokerMock.VerifyNoOtherCalls();
    }

}