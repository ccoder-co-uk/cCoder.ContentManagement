// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Net;
using cCoder.ContentManagement.Models.PageRendering;
using cCoder.ContentManagement.Rendering.Services.Processings;
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
        MarkupRenderProcessingService service = CreateMarkupRenderService();

        RenderSession session = new()
        {
            Request = new RenderRequest
            {
                Culture = attack,
                Theme = attack
            },
            Target = new RenderTarget
            {
                Scope = RenderScope.Page,
                ResourceKey = "Default",
                HeaderMarkup = $"<meta content=\"{tag}\"><div data-value='{tag}'>{tag}</div><script>const value = \"{tag}\";</script>",
                BodyMarkup = string.Empty,
                AllowHeaderContentTags = false,
                AllowBodyContentTags = true
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
            User = new PageRenderUser
            {
                Id = "Guest",
                AppPrivileges = new Dictionary<int, ISet<string>>()
            },
            Layout = new PageRenderLayout
            {
                HeaderHtml = $"<meta content=\"{tag}\"><div data-value='{tag}'>{tag}</div><script>const value = \"{tag}\";</script>"
            },
            Resources = [],
            ResourcesByLookup = new Dictionary<string, PageRenderResource>(),
            ComponentsByName = new Dictionary<string, PageRenderComponent>(),
            ScriptsByName = new Dictionary<string, PageRenderScript>(),
            MetadataResolver = unusedKey => string.Empty,
            CommonResourcesByLookup = new Dictionary<string, PageRenderResource>(),
            CommonComponentsByName = new Dictionary<string, PageRenderComponent>(),
            CommonScriptsByName = new Dictionary<string, PageRenderScript>()
        };

        // When
        RenderOutput result =
            service.RenderRenderSession(session: session).Output;

        // Then
        result.HeaderMarkup.Should()
            .NotContain(unexpected: attack);

        result.HeaderMarkup.Should()
            .Contain(expected: WebUtility.HtmlEncode(value: attack));

        result.HeaderMarkup.Should()
            .NotContain(unexpected: "<script>alert");
    }
}