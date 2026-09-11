// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Models;
using cCoder.ContentManagement.Services.Processings;
using cCoder.ContentManagement.Brokers;
using cCoder.ContentManagement.Rendering.Brokers;
using cCoder.ContentManagement.Rendering.Services.Foundations;
using cCoder.Data.Models.CMS;
using Moq;
using Xunit;

namespace cCoder.ContentManagement.Tests.Processings;

public sealed partial class CachedPageRenderProcessingServiceTests
{
    [Fact]
    public void ShouldMapCachedPageRenderOperation()
    {
        // Given
        PageRenderCacheOperation operation = new()
        {
            Cache = new PageRenderCache
            {
                AppId = 3,
                PageId = 17,
                Path = "/Documentation",
                Title = "Documentation",
                Header = "<style>.page { color: red; }</style>",
                Body = "<main /><script>start();</script>"
            },
            RenderOperation = new HttpPageRenderOperation
            {
                Context = new HttpPageRenderContext
                {
                    AppId = 3,
                    PageId = 17,
                    Culture = "en-GB",
                    Theme = "Default"
                }
            }
        };

        CachedPageRenderProcessingService service = new(
            markupRenderService: new MarkupRenderService(
                renderBroker: Mock.Of<IRenderBroker>(),
                regularExpressionBroker: new RegularExpressionBroker()));

        // When
        PageRenderCacheOperation result =
            service.RenderPageRenderCacheOperation(pageRenderCacheOperation: operation);

        // Then
        Assert.Same(
            expected: operation,
            actual: result);

        Assert.Equal(
            expected:
                "<main /><script nonce='[request[nonce]]'>start();</script>",
            actual: result.RenderOperation.Response.Page.BodyHtml);

        Assert.Equal(
            expected:
                "<style nonce='[request[nonce]]'>.page { color: red; }</style>",
            actual: result.RenderOperation.Response.Page.HeaderHtml);
    }
}