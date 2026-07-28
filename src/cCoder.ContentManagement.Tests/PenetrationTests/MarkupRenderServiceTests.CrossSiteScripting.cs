// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Net;
using cCoder.ContentManagement.Models.PageRendering;
using cCoder.ContentManagement.Models;
using cCoder.ContentManagement.Rendering.Services.Foundations;
using FluentAssertions;
using Xunit;

namespace cCoder.ContentManagement.Tests.PenetrationTests;

public partial class MarkupRenderServiceTests
{
    public static TheoryData<string, string> CrossSiteScriptingScenarios()
    {
        TheoryData<string, string> scenarios = new();

        string[] guestControlledTags =
        {
            "[[culture]]",
            "[[lang]]",
            "[page[path]]",
            "[page[url]]",
            "[theme[name]]"
        };

        string[] attacks =
        {
            "\"><img src=x onerror=alert(document.domain)>",
            "'><svg/onload=alert(document.domain)>",
            "\" autofocus onfocus=alert(document.domain) x=\"",
            "</script><script>alert(document.domain)</script>",
            "<iframe srcdoc=\"<script>alert(document.domain)</script>\">",
            "&quot;&gt;&lt;img src=x onerror=alert(document.domain)&gt;"
        };

        foreach (string tag in guestControlledTags)
        {
            foreach (string attack in attacks)
            {
                scenarios.Add(p1: tag, p2: attack);
            }
        }

        return scenarios;
    }

    [Theory]
    [MemberData(nameof(CrossSiteScriptingScenarios))]
    public void ShouldEncodeGuestControlledTagValuesAgainstCrossSiteScripting(
        string tag,
        string attack)
    {
        // Given
        MarkupRenderService service = CreateMarkupRenderService();

        PageRenderSession session = new()
        {
            Request = new PageRenderEngineRequest
            {
                Culture = attack,
                Theme = attack
            },
            Config = new ContentManagementConfiguration(),
            App = new PageRenderApp
            {
                Domain = "example.test",
                DefaultCulture = attack
            },
            Page = new PageRenderPage
            {
                ResourceKey = "Default",
                Path = attack
            },
            Layout = new PageRenderLayout
            {
                HeaderHtml = $"<meta content=\"{tag}\"><div data-value='{tag}'>{tag}</div><script>const value = \"{tag}\";</script>"
            }
        };

        // When
        PageRenderResult result =
            service.RenderPageRenderSession(session: session).Result;

        // Then
        result.HeaderHtml.Should()
            .NotContain(unexpected: attack);

        result.HeaderHtml.Should()
            .Contain(expected: WebUtility.HtmlEncode(value: attack));

        result.HeaderHtml.Should()
            .NotContain(unexpected: "<script>alert");
    }
}