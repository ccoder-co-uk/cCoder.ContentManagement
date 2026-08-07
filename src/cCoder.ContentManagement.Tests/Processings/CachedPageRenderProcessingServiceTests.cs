// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Models;
using cCoder.ContentManagement.Services.Processings;
using cCoder.Data.Models.CMS;
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
                Header = "<head />",
                Body = "<main />"
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

        CachedPageRenderProcessingService service = new();

        // When
        PageRenderCacheOperation result =
            service.RenderPageRenderCacheOperation(operation: operation);

        // Then
        Assert.Same(
            expected: operation,
            actual: result);

        Assert.Equal(
            expected: operation.Cache.Body,
            actual: result.RenderOperation.Response.Page.BodyHtml);
    }
}