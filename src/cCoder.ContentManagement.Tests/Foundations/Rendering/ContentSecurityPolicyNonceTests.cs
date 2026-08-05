// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Rendering.Services.Foundations;
using FluentAssertions;
using Xunit;

namespace cCoder.ContentManagement.Tests.Foundations.Rendering;

public sealed partial class ContentSecurityPolicyNonceTests
{
    [Fact]
    public void MarkShouldMarkInlineAndExternalScriptAndStyleElements()
    {
        // Given
        const string markup = "<style>.page { color: red; }</style><script src='/site.js'></script><script>start();</script>";

        // When
        string result = MarkupRenderService.MarkContentSecurityPolicyNonce(markup: markup);

        // Then
        result.Should()
            .Be(expected:
            "<style nonce='[request[nonce]]'>.page { color: red; }</style>" +
            "<script src='/site.js' nonce='[request[nonce]]'></script>" +
            "<script nonce='[request[nonce]]'>start();</script>");
    }

    [Fact]
    public void MarkShouldReplaceExistingNonceAndRemainIdempotent()
    {
        // Given
        const string markup = "<script nonce=\"stale\">start();</script>";

        // When
        string firstResult = MarkupRenderService.MarkContentSecurityPolicyNonce(markup: markup);
        string secondResult = MarkupRenderService.MarkContentSecurityPolicyNonce(markup: firstResult);

        // Then
        firstResult.Should()
            .Be(expected: "<script nonce='[request[nonce]]'>start();</script>");

        secondResult.Should()
            .Be(expected: firstResult);
    }

    [Fact]
    public void MarkShouldNotAlterScriptTextThatContainsElementLikeText()
    {
        // Given
        const string markup = "<script>const example = \"<style>not markup</style>\";</script><p>content</p>";

        // When
        string result = MarkupRenderService.MarkContentSecurityPolicyNonce(markup: markup);

        // Then
        result.Should()
            .Be(expected:
            "<script nonce='[request[nonce]]'>const example = \"<style>not markup</style>\";</script><p>content</p>");
    }

    [Fact]
    public void MarkShouldRespectGreaterThanCharactersInsideQuotedAttributes()
    {
        // Given
        const string markup = "<script data-example='a > b'>start();</script>";

        // When
        string result = MarkupRenderService.MarkContentSecurityPolicyNonce(markup: markup);

        // Then
        result.Should()
            .Be(expected:
            "<script data-example='a > b' nonce='[request[nonce]]'>start();</script>");
    }

    [Fact]
    public void MarkShouldLeaveStyleAndEventHandlerAttributesUnchanged()
    {
        // Given
        const string markup = "<button style='color:red' onclick='start()'>Start</button>";

        // When
        string result = MarkupRenderService.MarkContentSecurityPolicyNonce(markup: markup);

        // Then
        result.Should()
            .Be(expected: markup);
    }
}