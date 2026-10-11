// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System;
using System.Diagnostics;
using System.Linq;
using cCoder.ContentManagement.Brokers;
using cCoder.ContentManagement.Services.Foundations.Rendering;
using Xunit;

namespace cCoder.ContentManagement.Tests.Foundations.Rendering;

public sealed partial class CachedPageRenderServiceTests
{
    [Fact]
    public void ShouldReuseCompiledExpressionsWhenMarkingCachedPageMarkup()
    {
        // Given
        string markup = string.Concat(
            values: Enumerable.Repeat(
                element: "<script nonce='stale'>run();</script><style>.item{display:block}</style>",
                count: 500));

        CachedPageRenderService service = new(
            regularExpressionBroker: new RegularExpressionBroker());

        Stopwatch stopwatch = Stopwatch.StartNew();

        // When
        string result = service.MarkContentSecurityPolicyNonce(markup: markup);
        stopwatch.Stop();

        // Then
        Assert.DoesNotContain(
            expectedSubstring: "nonce='stale'",
            actualString: result);

        Assert.Contains(
            expectedSubstring: "nonce='[request[nonce]]'",
            actualString: result);

        Assert.True(
            condition: stopwatch.Elapsed < TimeSpan.FromSeconds(value: 1.5),
            userMessage: $"Cached markup normalization took {stopwatch.Elapsed}.");
    }
}