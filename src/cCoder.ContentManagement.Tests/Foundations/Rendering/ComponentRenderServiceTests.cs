// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Brokers;
using cCoder.ContentManagement.Brokers.Storages;
using cCoder.ContentManagement.Models.Rendering;
using cCoder.ContentManagement.Rendering.Brokers;
using cCoder.ContentManagement.Services.Foundations.Rendering;
using cCoder.ContentManagement.Tests.Brokers.Rendering;
using FluentAssertions;
using Moq;
using Xunit;

namespace cCoder.ContentManagement.Tests.Foundations.Rendering;

public sealed partial class ComponentRenderServiceTests
{
    [Fact]
    public void FileContent_WhenRendered_ShouldReturnLatestUtf8Text()
    {
        // Given
        const int appId = 123;
        const string path = "documents/example.html";
        const string expectedContent = "<p>latest content</p>";
        Mock<IRenderFileContentBroker> renderFileContentBrokerMock = new(MockBehavior.Strict);

        renderFileContentBrokerMock.Setup(expression: broker =>
            broker.GetLatestTextContent(appId: appId, path: path))
            .Returns(value: expectedContent);

        IComponentRenderService componentRenderService = new ComponentRenderService(
            contentRenderBroker: new TestContentRenderBroker(
                renderFileContentBroker: renderFileContentBrokerMock.Object),
            workflowExecutionBroker: Mock.Of<cCoder.ContentManagement.Brokers.IWorkflowExecutionBroker>(),
            cacheBroker: Mock.Of<cCoder.ContentManagement.Brokers.Caching.ICacheBroker>(),
            jsonBroker: Mock.Of<IJsonBroker>(),
            regularExpressionBroker: Mock.Of<IRegularExpressionBroker>());

        // When
        ComponentRenderFoundationOperation operation =
            componentRenderService.GetLatestTextContentComponentRenderFoundationOperation(
                componentRenderFoundationOperation: new()
                {
                    AppId = appId,
                    Path = path
                });

        // Then
        operation.Content.Should()
            .Be(expected: expectedContent);

        renderFileContentBrokerMock.VerifyAll();
    }
}