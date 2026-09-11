// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Brokers;
using cCoder.ContentManagement.Brokers.Storages;
using cCoder.ContentManagement.Rendering.Brokers;
using cCoder.ContentManagement.Rendering.Services.Foundations;
using cCoder.ContentManagement.Rendering.Services.Processings;
using Moq;

namespace cCoder.ContentManagement.Tests.PenetrationTests;

public partial class MarkupRenderServiceTests
{
    private static MarkupRenderProcessingService CreateMarkupRenderService()
    {
        RegularExpressionBroker regularExpressionBroker = new();

        return new(
            markupRenderService: new MarkupRenderService(
                componentReaderBroker: Mock.Of<IComponentReaderBroker>(),
                scriptReaderBroker: Mock.Of<IScriptReaderBroker>(),
                renderFileContentBroker: Mock.Of<IRenderFileContentBroker>(),
                jsonBroker: Mock.Of<IJsonBroker>(),
                workflowExecutionBroker: Mock.Of<IWorkflowExecutionBroker>(),
                regularExpressionBroker: regularExpressionBroker),
            jsonBroker: Mock.Of<IJsonBroker>());
    }
}