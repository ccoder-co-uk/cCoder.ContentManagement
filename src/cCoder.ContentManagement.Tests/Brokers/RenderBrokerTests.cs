// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Exposures;
using cCoder.ContentManagement.Models.PageRendering;
using cCoder.ContentManagement.Rendering.Brokers;
using cCoder.ContentManagement.Rendering.Services.Orchestrations;
using FluentAssertions;
using Moq;
using Xunit;

namespace cCoder.ContentManagement.Tests.Brokers;

public sealed partial class RenderBrokerTests
{
    [Fact]
    public void ShouldPassRenderSessionThroughExposureToOrchestration()
    {
        // Given
        RenderSession session = new();
        Mock<IRenderOrchestrationService> renderOrchestrationService = new();

        renderOrchestrationService
            .Setup(expression: service => service.RenderRenderSession(
                session: session))
            .Returns(value: session);

        RenderBroker broker = new(
            renderSessionManager: new RenderSessionManager(
                renderOrchestrationService: renderOrchestrationService.Object));

        // When
        RenderSession result = broker.RenderRenderSession(
            renderSession: session);

        // Then
        result.Should()
            .BeSameAs(expected: session);

        renderOrchestrationService.Verify(
            expression: service => service.RenderRenderSession(
                session: session),
            times: Times.Once);
    }
}