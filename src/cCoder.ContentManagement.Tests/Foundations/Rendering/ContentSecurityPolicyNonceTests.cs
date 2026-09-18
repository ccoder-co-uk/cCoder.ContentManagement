// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Rendering.Services.Foundations;
using cCoder.ContentManagement.Brokers;
using cCoder.ContentManagement.Brokers.Storages;
using cCoder.ContentManagement.Rendering.Brokers;
using FluentAssertions;
using Moq;
using Xunit;
using cCoder.ContentManagement.Tests.Brokers.Rendering;

namespace cCoder.ContentManagement.Tests.Foundations.Rendering;

public sealed partial class ContentSecurityPolicyNonceTests
{
    [Fact]
    public void MarkShouldMarkInlineAndExternalScriptAndStyleElements()
    {
        // Given
        const string markup = "<style>.page { color: red; }</style><script src='/site.js'></script><script>start();</script>";

        // When
        string result = CreateService()
            .MarkContentSecurityPolicyNonce(markup: markup);

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
        MarkupRenderService service = CreateService();
        string firstResult = service.MarkContentSecurityPolicyNonce(markup: markup);
        string secondResult = service.MarkContentSecurityPolicyNonce(markup: firstResult);

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
        string result = CreateService()
            .MarkContentSecurityPolicyNonce(markup: markup);

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
        string result = CreateService()
            .MarkContentSecurityPolicyNonce(markup: markup);

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
        string result = CreateService()
            .MarkContentSecurityPolicyNonce(markup: markup);

        // Then
        result.Should()
            .Be(expected: markup);
    }

    private static MarkupRenderService CreateService() =>
        new(
            contentRenderBroker: new TestContentRenderBroker(),
            workflowExecutionBroker: Mock.Of<cCoder.ContentManagement.Brokers.IWorkflowExecutionBroker>(),
            cacheBroker: Mock.Of<cCoder.ContentManagement.Brokers.Caching.ICacheBroker>(),
            jsonBroker: Mock.Of<IJsonBroker>(),
            regularExpressionBroker: new RegularExpressionBroker(),
            renderingUtilityBroker:
                new cCoder.ContentManagement.Brokers.Rendering.RenderingUtilityBroker());
}