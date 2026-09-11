// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Text;
using cCoder.ContentManagement.Brokers.Storages;
using cCoder.ContentManagement.Services.Foundations.Rendering;
using FluentAssertions;
using Moq;
using Xunit;

namespace cCoder.ContentManagement.Tests.Foundations.Rendering;

public sealed class ComponentRenderServiceTests
{
    [Fact]
    public void FileContent_WhenRendered_ShouldReturnLatestUtf8Text()
    {
        const int appId = 123;
        const string path = "documents/example.html";
        const string expectedContent = "<p>latest content</p>";
        Mock<IRenderFileContentBroker> renderFileContentBrokerMock = new(MockBehavior.Strict);

        renderFileContentBrokerMock.Setup(expression: broker => broker.GetLatestRawData(appId, path))
            .Returns(Encoding.UTF8.GetBytes(expectedContent));

        IComponentRenderService componentRenderService = new ComponentRenderService(
            renderFileContentBroker: renderFileContentBrokerMock.Object);

        string actualContent = componentRenderService.GetLatestTextContent(appId: appId, path: path);

        actualContent.Should().Be(expectedContent);
        renderFileContentBrokerMock.VerifyAll();
    }
}