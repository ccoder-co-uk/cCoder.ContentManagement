// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Rendering.Brokers;
using cCoder.ContentManagement.Rendering.Services.Foundations;
using cCoder.ContentManagement.Models.PageRendering;
using Moq;
using Xunit;

namespace cCoder.ContentManagement.Tests.Foundations.PageRendering;

public sealed partial class CommonObjectCacheServiceTests
{
    [Fact]
    public void ShouldEnsureCommonObjectsAreAvailableBeforeReadingSlice()
    {
        // Given
        Mock<ICommonObjectReaderBroker> broker = new();
        MockSequence sequence = new();

        broker.InSequence(sequence: sequence)
            .Setup(expression: item => item.EnsureAvailable());

        broker.InSequence(sequence: sequence)
            .Setup(expression: item => item.GetResourcesByLookup())
            .Returns(value: new Dictionary<string, PageRenderResource>());

        broker.InSequence(sequence: sequence)
            .Setup(expression: item => item.GetComponentsByName())
            .Returns(value: new Dictionary<string, PageRenderComponent>());

        broker.InSequence(sequence: sequence)
            .Setup(expression: item => item.GetScriptsByName())
            .Returns(value: new Dictionary<string, PageRenderScript>());

        broker.InSequence(sequence: sequence)
            .Setup(expression: item => item.GetStylesByName())
            .Returns(value: new Dictionary<string, PageRenderStyle>());

        CommonObjectCacheService service = new(broker: broker.Object);

        // When
        _ = service.GetPageCacheSlice();

        // Then
        broker.VerifyAll();
    }
}