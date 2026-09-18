// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Brokers;
using cCoder.ContentManagement.Brokers.Storages;
using cCoder.ContentManagement.Rendering.Brokers;
using cCoder.ContentManagement.Rendering.Services.Foundations;
using cCoder.ContentManagement.Rendering.Services.Processings;
using cCoder.ContentManagement.Tests.Brokers.Rendering;
using Moq;

namespace cCoder.ContentManagement.Tests.PenetrationTests;

public partial class MarkupRenderServiceTests
{
    private static MarkupRenderProcessingService CreateMarkupRenderService()
    {
        RegularExpressionBroker regularExpressionBroker = new();

        return new(
            markupRenderService: new MarkupRenderService(
                contentRenderBroker: new TestContentRenderBroker(),
                workflowExecutionBroker: Mock.Of<IWorkflowExecutionBroker>(),
                cacheBroker: Mock.Of<cCoder.ContentManagement.Brokers.Caching.ICacheBroker>(),
                jsonBroker: Mock.Of<IJsonBroker>(),
                regularExpressionBroker: regularExpressionBroker,
                renderingUtilityBroker:
                    new cCoder.ContentManagement.Brokers.Rendering.RenderingUtilityBroker()));
    }
}