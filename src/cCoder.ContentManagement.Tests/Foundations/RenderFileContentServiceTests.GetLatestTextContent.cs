using cCoder.Data.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Packaging;
using cCoder.Data.Models.Security;
using ComponentRenderParams = cCoder.ContentManagement.Models.ComponentRenderParams;
using Config = cCoder.ContentManagement.Models.Config;
using PageRenderParams = cCoder.ContentManagement.Models.PageRenderParams;
using PageRoleInfo = cCoder.ContentManagement.Models.PageRoleInfo;
using RenderParams = cCoder.ContentManagement.Models.RenderParams;
using RenderResult = cCoder.ContentManagement.Models.RenderResult;
using TemplateRenderParams = cCoder.ContentManagement.Models.TemplateRenderParams;
using System.ComponentModel.DataAnnotations;
using System.Text;
using FluentAssertions;
using Moq;
using Xunit;


namespace cCoder.Core.Services.Tests.CMS.Foundations;

public partial class RenderFileContentServiceTests
{
    [Fact]
    public void ShouldThrowValidationExceptionOnGetLatestTextContentWhenAppIdIsInvalid()
    {
        Action action = () => renderFileContentService.GetLatestTextContent(0, "/path/file.txt");

        action.Should().Throw<ValidationException>()
            .WithMessage("appId must be greater than 0.");

        renderFileContentBrokerMock.VerifyNoOtherCalls();
    }

    [Fact]
    public void ShouldThrowValidationExceptionOnGetLatestTextContentWhenPathIsInvalid()
    {
        Action action = () => renderFileContentService.GetLatestTextContent(1, " ");

        action.Should().Throw<ValidationException>()
            .WithMessage("path is required.");

        renderFileContentBrokerMock.VerifyNoOtherCalls();
    }

    [Fact]
    public void ShouldReturnEmptyStringOnGetLatestTextContentWhenRawDataIsMissing()
    {
        renderFileContentBrokerMock
            .Setup(broker => broker.GetLatestRawData(1, "/assets/file.txt"))
            .Returns([]);

        string result = renderFileContentService.GetLatestTextContent(1, "/assets/file.txt");

        result.Should().BeEmpty();
        renderFileContentBrokerMock.Verify(broker => broker.GetLatestRawData(1, "/assets/file.txt"), Times.Once);
        renderFileContentBrokerMock.VerifyNoOtherCalls();
    }

    [Fact]
    public void ShouldReturnDecodedTextOnGetLatestTextContent()
    {
        string expected = "rendered file content";
        byte[] rawData = Encoding.UTF8.GetBytes(expected);

        renderFileContentBrokerMock
            .Setup(broker => broker.GetLatestRawData(7, "/assets/content.txt"))
            .Returns(rawData);

        string result = renderFileContentService.GetLatestTextContent(7, "/assets/content.txt");

        result.Should().Be(expected);
        renderFileContentBrokerMock.Verify(broker => broker.GetLatestRawData(7, "/assets/content.txt"), Times.Once);
        renderFileContentBrokerMock.VerifyNoOtherCalls();
    }
}




