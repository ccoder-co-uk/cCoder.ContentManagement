// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Exposures;
using cCoder.Data.Models.CMS;
using FluentAssertions;
using Moq;
using Xunit;

namespace cCoder.Core.Services.Tests.CMS.Aggregations;

public partial class PageRenderAggregationServiceTests
{
    [Fact]
    public void ShouldEncodeErrorPageTagsAgainstReflectedCrossSiteScripting()
    {
        // Given
        const string attack =
            "\"><img src=x onerror=alert(document.domain)>";

        App app = CreateApp();

        PageRenderRequest request = new()
        {
            Host = app.Domain,
            RequestUrl = "https://demo.local/" + attack,
            Exception = new InvalidOperationException(message: attack)
        };

        appOrchestrationServiceMock
            .Setup(expression: service =>
                service.GetByDomainApp(
                    domain: app.Domain,
                    ignoreFilters: true))
            .Returns(value: app);

        appOrchestrationServiceMock
            .Setup(expression: service =>
                service.GetAllApp(ignoreFilters: false))
            .Returns(value: new[] { app }.AsQueryable());

        SetupRenderResult(
            renderResult: CreateRenderResult(
                bodyHtml:
                "<p>[problem[message]]</p><pre>[problem[detail]]</pre><a href='[problem[url]]'>URL</a>"));

        // When
        PageRenderResponse response =
            aggregationService.RenderErrorPageRenderRequestPageRenderResponse(
                request: request);

        // Then
        response.Page.BodyHtml.Should()
            .NotContain(unexpected: attack);

        response.Page.BodyHtml.Should()
            .NotContain(unexpected: "<img");

        response.Page.BodyHtml.Should()
            .Contain(expected: "&quot;&gt;&lt;img");
    }
}