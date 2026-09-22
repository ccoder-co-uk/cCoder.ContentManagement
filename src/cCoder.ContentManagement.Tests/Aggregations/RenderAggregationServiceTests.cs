// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Models;
using cCoder.ContentManagement.Models.Exceptions;
using cCoder.ContentManagement.Brokers;
using cCoder.ContentManagement.Services.Aggregations;
using cCoder.ContentManagement.Services.Orchestrations;
using cCoder.ContentManagement.Services.Orchestrations.Caching;
using cCoder.ContentManagement.Services.Orchestrations.PageContexts;
using cCoder.Data.Models.Security;
using FluentAssertions;
using Moq;
using Xunit;

namespace cCoder.ContentManagement.Tests.Aggregations;

public sealed partial class RenderAggregationServiceTests
{
    [Theory]
    [InlineData(
        (int)HttpPageRenderFailure.PageNotFound,
        typeof(PageNotFoundException))]
    [InlineData(
        (int)HttpPageRenderFailure.PageAccessDenied,
        typeof(PageAccessSecurityException))]
    public async Task ShouldRestoreRequestFailureAfterRenderEventCompletesAsync(
        int failureValue,
        Type expectedExceptionType)
    {
        // Given
        HttpPageRenderFailure failure =
            (HttpPageRenderFailure)failureValue;

        HttpPageRenderContext context = new();
        Mock<IPageContextOrchestrationService> contextService = new();
        Mock<IRenderEventOrchestrationService> renderEventService = new();

        contextService.Setup(expression: service =>
                service.ResolvePageRenderContextAsync())
            .Returns(value: ValueTask.FromResult(result: context));

        renderEventService.Setup(expression: service =>
                service.RaiseHttpPageRenderOperationRenderRequestAsync(
                    httpPageRenderOperation:
                        It.IsAny<HttpPageRenderOperation>()))
            .Callback<HttpPageRenderOperation>(action: operation =>
                operation.Failure = failure)
            .Returns(value: ValueTask.CompletedTask);

        RenderAggregationService service = new(
            pageContextOrchestrationService: contextService.Object,
            cachedPageRenderOrchestrationService:
                Mock.Of<ICachedPageRenderOrchestrationService>(),
            renderEventOrchestrationService: renderEventService.Object,
            renderDataOrchestrationService:
                Mock.Of<IRenderDataOrchestrationService>(),
            templateRenderOrchestrationService:
                Mock.Of<ITemplateRenderOrchestrationService>(),
            componentRenderOrchestrationService:
                Mock.Of<IComponentRenderOrchestrationService>(),
            commonObjectCacheOrchestrationService:
                Mock.Of<ICommonObjectCacheOrchestrationService>());

        // When
        Exception actualException = await Record.ExceptionAsync(
            testCode: () => service.RenderPageRenderResultAsync()
                .AsTask());

        // Then
        Assert.IsType(
            expectedType: expectedExceptionType,
            @object: actualException);

        renderEventService.VerifyAll();
    }

    private static Mock<IRenderDataOrchestrationService>
        CreateRenderDataOrchestrationServiceMock()
    {
        Mock<IRenderDataOrchestrationService> service = new();

        service.Setup(expression: candidate => candidate.HtmlEncode(
                value: It.IsAny<string>()))
            .Returns(valueFunction: (string value) =>
                System.Net.WebUtility.HtmlEncode(value: value));

        return service;
    }

    [Fact]
    public async Task ShouldReturnCachedResponseWithoutRenderingUncachedAsync()
    {
        // Given
        HttpPageRenderContext context = new()
        {
            PageId = 7,
            Nonce = "request-nonce",
            Culture = "en-GB",
            User = new User
            {
                Id = "Paul",
                DisplayName = "Paul & Ward",
                Email = "paul.ward@ccoder.co.uk"
            }
        };

        PageRenderResponse expectedResponse = new()
        {
            Page = new PageRenderResult
            {
                HeaderHtml =
                    "<style nonce='[request[nonce]]'></style>" +
                    "{{ccoder-runtime-date}}",
                BodyHtml =
                    "<script nonce='[request[nonce]]'>" +
                    "const user = {{ccoder-runtime-user}};</script>" +
                    "{{ccoder-runtime-display-name}} " +
                    "({{ccoder-runtime-login-link}})"
            }
        };

        Mock<IPageContextOrchestrationService> contextService = new();
        Mock<ICachedPageRenderOrchestrationService> cachedService = new();
        Mock<IRenderEventOrchestrationService> renderEventService = new();
        Mock<IRenderDataOrchestrationService> jsonService = CreateRenderDataOrchestrationServiceMock();
        Mock<ICommonObjectCacheOrchestrationService> cacheService = new();

        cacheService.Setup(
            expression: service => service.EnsureAvailable());

        jsonService.Setup(expression: service =>
            service.SerializeRuntimeValue(value: It.IsAny<object>()))
            .Returns(valueFunction: (object value) =>
                new JsonBroker().Serialize(value: value));

        contextService.Setup(expression: service =>
                service.ResolvePageRenderContextAsync())
            .Returns(value: ValueTask.FromResult(result: context));

        cachedService.Setup(expression: service =>
                service.RenderHttpPageRenderOperation(
                    operation: It.Is<HttpPageRenderOperation>(match:
                        operation => operation.Context == context)))
            .Returns(valueFunction: (HttpPageRenderOperation operation) =>
            {
                operation.Response = expectedResponse;
                return operation;
            });

        RenderAggregationService service = new(
            pageContextOrchestrationService: contextService.Object,
            cachedPageRenderOrchestrationService: cachedService.Object,
            renderEventOrchestrationService: renderEventService.Object,
            renderDataOrchestrationService: jsonService.Object,
            templateRenderOrchestrationService:
                Mock.Of<ITemplateRenderOrchestrationService>(),
            componentRenderOrchestrationService:
                Mock.Of<IComponentRenderOrchestrationService>(),
            commonObjectCacheOrchestrationService:
                cacheService.Object);

        // When
        RenderResult result = await service
            .RenderPageRenderResultAsync();

        PageRenderResponse actualResponse = result.PageResponse;

        // Then
        actualResponse
            .Should()
            .BeSameAs(expected: expectedResponse);

        actualResponse.Page.HeaderHtml
            .Should()
            .Contain(expected: "nonce='request-nonce'");

        actualResponse.Page.BodyHtml
            .Should()
            .Contain(expected: "nonce='request-nonce'");

        actualResponse.Page.BodyHtml
            .Should()
            .Contain(expected: "Paul &amp; Ward (" +
                "<a name='logout' href=''>Logout</a>)");

        actualResponse.Page.BodyHtml
            .Should()
            .Contain(expected: "\"Id\":\"Paul\"");

        actualResponse.Page.BodyHtml
            .Should()
            .NotContain(unexpected: "{{ccoder-runtime-");

        jsonService.Verify(
            expression: service => service.SerializeRuntimeValue(
                value: It.IsAny<object>()),
            times: Times.Once);

        cacheService.Verify(
            expression: service => service.EnsureAvailable(),
            times: Times.Once);

        renderEventService.VerifyNoOtherCalls();
    }

    [Theory]
    [InlineData(true, 7)]
    [InlineData(false, null)]
    public async Task ShouldRenderUncachedOnlyOnceWhenCacheIsNotApplicableAsync(
        bool edit,
        int? pageId)
    {
        // Given
        HttpPageRenderContext context = new()
        {
            Edit = edit,
            PageId = pageId,
            Nonce = "request-nonce"
        };

        PageRenderResponse expectedResponse = new()
        {
            Page = new PageRenderResult
            {
                HeaderHtml = string.Empty,
                BodyHtml = string.Empty
            }
        };

        Mock<IPageContextOrchestrationService> contextService = new();
        Mock<ICachedPageRenderOrchestrationService> cachedService = new();
        Mock<IRenderEventOrchestrationService> renderEventService = new();
        Mock<IRenderDataOrchestrationService> jsonService = CreateRenderDataOrchestrationServiceMock();

        jsonService.Setup(expression: service =>
            service.SerializeRuntimeValue(value: It.IsAny<object>()))
            .Returns(valueFunction: (object value) =>
                new JsonBroker().Serialize(value: value));

        contextService.Setup(expression: service =>
                service.ResolvePageRenderContextAsync())
            .Returns(value: ValueTask.FromResult(result: context));

        renderEventService.Setup(expression: service =>
                service.RaiseHttpPageRenderOperationRenderRequestAsync(
                    httpPageRenderOperation: It.IsAny<HttpPageRenderOperation>()))
            .Callback<HttpPageRenderOperation>(action: operation =>
            {
                operation.Response = expectedResponse;
            })
            .Returns(value: ValueTask.CompletedTask);

        RenderAggregationService service = new(
            pageContextOrchestrationService: contextService.Object,
            cachedPageRenderOrchestrationService: cachedService.Object,
            renderEventOrchestrationService: renderEventService.Object,
            renderDataOrchestrationService: jsonService.Object,
            templateRenderOrchestrationService:
                Mock.Of<ITemplateRenderOrchestrationService>(),
            componentRenderOrchestrationService:
                Mock.Of<IComponentRenderOrchestrationService>(),
            commonObjectCacheOrchestrationService:
                Mock.Of<ICommonObjectCacheOrchestrationService>());

        // When
        RenderResult result = await service
            .RenderPageRenderResultAsync();

        PageRenderResponse actualResponse = result.PageResponse;

        // Then
        actualResponse
            .Should()
            .BeSameAs(expected: expectedResponse);

        cachedService.VerifyNoOtherCalls();

        renderEventService.Verify(
            expression: item => item.RaiseHttpPageRenderOperationRenderRequestAsync(
                httpPageRenderOperation: It.IsAny<HttpPageRenderOperation>()),
            times: Times.Once);
    }

    [Fact]
    public async Task ShouldRenderTemplateUsingResolvedRequestContextAsync()
    {
        // Given
        HttpPageRenderContext context = new()
        {
            AppId = 17,
            Culture = "en-GB"
        };

        TemplateRenderResult expected = new() { Content = "template" };
        Mock<IPageContextOrchestrationService> contextService = new();
        Mock<ITemplateRenderOrchestrationService> templateService = new();

        contextService.Setup(expression: service =>
                service.ResolvePageRenderContextAsync())
            .Returns(value: ValueTask.FromResult(result: context));

        templateService.Setup(expression: service =>
                service.RenderTemplateRenderResult(
                    appId: 17,
                    name: "Welcome",
                    culture: "en-GB",
                    model: It.IsAny<object>()))
            .Returns(value: expected);

        RenderAggregationService service = new(
            pageContextOrchestrationService: contextService.Object,
            cachedPageRenderOrchestrationService:
                Mock.Of<ICachedPageRenderOrchestrationService>(),
            renderEventOrchestrationService:
                Mock.Of<IRenderEventOrchestrationService>(),
            renderDataOrchestrationService:
                Mock.Of<IRenderDataOrchestrationService>(),
            templateRenderOrchestrationService: templateService.Object,
            componentRenderOrchestrationService:
                Mock.Of<IComponentRenderOrchestrationService>(),
            commonObjectCacheOrchestrationService:
                Mock.Of<ICommonObjectCacheOrchestrationService>());

        // When
        RenderResult actual = await service
            .RenderTemplateRenderResultAsync(
                name: "Welcome",
                model: new { Name = "Paul" });

        // Then
        actual
            .Should()
            .BeSameAs(expected: expected);

        templateService.VerifyAll();
    }

    [Fact]
    public async Task ShouldRenderComponentUsingResolvedRequestContextAsync()
    {
        // Given
        HttpPageRenderContext context = new()
        {
            AppId = 17,
            Culture = "en-GB",
            Theme = "Default"
        };

        ComponentRenderResult expected = new() { Content = "component" };
        Mock<IPageContextOrchestrationService> contextService = new();
        Mock<IComponentRenderOrchestrationService> componentService = new();

        contextService.Setup(expression: service =>
                service.ResolvePageRenderContextAsync())
            .Returns(value: ValueTask.FromResult(result: context));

        componentService.Setup(expression: service =>
                service.RenderComponentRenderResult(
                    appId: 17,
                    name: "Hero",
                    culture: "en-GB",
                    theme: "default"))
            .Returns(value: expected);

        RenderAggregationService service = new(
            pageContextOrchestrationService: contextService.Object,
            cachedPageRenderOrchestrationService:
                Mock.Of<ICachedPageRenderOrchestrationService>(),
            renderEventOrchestrationService:
                Mock.Of<IRenderEventOrchestrationService>(),
            renderDataOrchestrationService:
                Mock.Of<IRenderDataOrchestrationService>(),
            templateRenderOrchestrationService:
                Mock.Of<ITemplateRenderOrchestrationService>(),
            componentRenderOrchestrationService: componentService.Object,
            commonObjectCacheOrchestrationService:
                Mock.Of<ICommonObjectCacheOrchestrationService>());

        // When
        RenderResult actual = await service
            .RenderComponentRenderResultAsync(name: "Hero");

        // Then
        actual
            .Should()
            .BeSameAs(expected: expected);

        componentService.VerifyAll();
    }
}