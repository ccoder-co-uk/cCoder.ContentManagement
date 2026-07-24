// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

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
        Action action = () => renderFileContentService.GetLatestTextContent(appId: 0, path: "/path/file.txt");

        action.Should()
            .Throw<ValidationException>()
            .WithMessage(expectedWildcardPattern: "appId must be greater than 0.");

        renderFileContentBrokerMock.VerifyNoOtherCalls();
    }

    [Fact]
    public void ShouldThrowValidationExceptionOnGetLatestTextContentWhenPathIsInvalid()
    {
        Action action = () => renderFileContentService.GetLatestTextContent(appId: 1, path: " ");

        action.Should()
            .Throw<ValidationException>()
            .WithMessage(expectedWildcardPattern: "path is required.");

        renderFileContentBrokerMock.VerifyNoOtherCalls();
    }

    [Fact]
    public void ShouldReturnEmptyStringOnGetLatestTextContentWhenRawDataIsMissing()
    {
        renderFileContentBrokerMock
            .Setup(expression: broker => broker.GetLatestRawData(appId: 1, path: "/assets/file.txt"))
            .Returns(value: []);

        string result = renderFileContentService.GetLatestTextContent(appId: 1, path: "/assets/file.txt");

        result.Should()
            .BeEmpty();

        renderFileContentBrokerMock.Verify(expression: broker => broker.GetLatestRawData(appId: 1, path: "/assets/file.txt"), times: Times.Once);
        renderFileContentBrokerMock.VerifyNoOtherCalls();
    }

    [Fact]
    public void ShouldReturnDecodedTextOnGetLatestTextContent()
    {
        string expected = "rendered file content";
        byte[] rawData = Encoding.UTF8.GetBytes(s: expected);

        renderFileContentBrokerMock
            .Setup(expression: broker => broker.GetLatestRawData(appId: 7, path: "/assets/content.txt"))
            .Returns(value: rawData);

        string result = renderFileContentService.GetLatestTextContent(appId: 7, path: "/assets/content.txt");

        result.Should()
            .Be(expected: expected);

        renderFileContentBrokerMock.Verify(expression: broker => broker.GetLatestRawData(appId: 7, path: "/assets/content.txt"), times: Times.Once);
        renderFileContentBrokerMock.VerifyNoOtherCalls();
    }
}