// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Brokers;
using cCoder.ContentManagement.Brokers.Storages;
using cCoder.ContentManagement.Rendering.Brokers;
using cCoder.ContentManagement.Rendering.Services.Foundations;
using cCoder.ContentManagement.Rendering.Services.Processings;
using cCoder.ContentManagement.Services.Processings.PageRendering;
using Moq;

namespace cCoder.ContentManagement.Tests.PenetrationTests;

public partial class MarkupRenderServiceTests
{
    private static MarkupRenderProcessingService CreateMarkupRenderService()
    {
        RegularExpressionBroker regularExpressionBroker = new();

        return new(
            markupRenderService: new MarkupRenderService(
            renderBroker: new RenderBroker(
                tagHandlers:
                [
                    new CultureLinkTagHandlingProcessingService(regularExpressionBroker),
                    new MetadataTagHandlingProcessingService(regularExpressionBroker),
                    new NavigationTagHandlingProcessingService(regularExpressionBroker),
                    new ContentTagHandlingProcessingService(regularExpressionBroker),
                    new ComponentTagHandlingProcessingService(
                        componentReaderBroker:
                            Mock.Of<IComponentReaderBroker>(),
                        regularExpressionBroker: regularExpressionBroker),
                    new ScriptTagHandlingProcessingService(
                        scriptReaderBroker:
                            Mock.Of<IScriptReaderBroker>(),
                        regularExpressionBroker: regularExpressionBroker),
                    new StyleTagHandlingProcessingService(regularExpressionBroker),
                    new ReplacementTagHandlingProcessingService(),
                    new DmsTagHandlingProcessingService(
                        renderFileContentBroker:
                            Mock.Of<IRenderFileContentBroker>(),
                        regularExpressionBroker: regularExpressionBroker),
                    new ResourceTagHandlingProcessingService(regularExpressionBroker),
                    new ExecuteTagHandlingProcessingService(
                        jsonBroker: Mock.Of<IJsonBroker>(),
                        workflowExecutionBroker:
                            Mock.Of<IWorkflowExecutionBroker>(),
                        regularExpressionBroker: regularExpressionBroker)
                ]),
                regularExpressionBroker: regularExpressionBroker),
            jsonBroker: Mock.Of<IJsonBroker>());
    }
}