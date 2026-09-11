// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Text;
using cCoder.ContentManagement.Brokers;
using cCoder.ContentManagement.Brokers.Storages;
using cCoder.ContentManagement.Models.Rendering;
using cCoder.ContentManagement.Rendering.Brokers;
using cCoder.ContentManagement.Services.Foundations.Rendering;
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
            broker.GetLatestRawData(appId: appId, path: path))
            .Returns(value: Encoding.UTF8.GetBytes(s: expectedContent));

        IComponentRenderService componentRenderService = new ComponentRenderService(
            metadataReaderBroker: Mock.Of<IMetadataReaderBroker>(),
            commonObjectReaderBroker: Mock.Of<ICommonObjectReaderBroker>(),
            jsonBroker: Mock.Of<IJsonBroker>(),
            workflowExecutionBroker: Mock.Of<IWorkflowExecutionBroker>(),
            regularExpressionBroker: Mock.Of<IRegularExpressionBroker>(),
            renderFileContentBroker: renderFileContentBrokerMock.Object,
            appBroker: Mock.Of<IAppBroker>(),
            componentBroker: Mock.Of<IComponentBroker>(),
            resourceBroker: Mock.Of<IResourceBroker>(),
            scriptBroker: Mock.Of<IScriptBroker>());

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