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
    private static MarkupRenderProcessingService CreateMarkupRenderService() =>
        new(
            markupRenderService: new MarkupRenderService(
            renderBroker: new RenderBroker(
                tagHandlers:
                [
                    new CultureLinkTagHandlingProcessingService(),
                    new MetadataTagHandlingProcessingService(),
                    new NavigationTagHandlingProcessingService(),
                    new ContentTagHandlingProcessingService(),
                    new ComponentTagHandlingProcessingService(
                        componentReaderBroker:
                            Mock.Of<IComponentReaderBroker>()),
                    new ScriptTagHandlingProcessingService(
                        scriptReaderBroker:
                            Mock.Of<IScriptReaderBroker>()),
                    new ReplacementTagHandlingProcessingService(),
                    new DmsTagHandlingProcessingService(
                        renderFileContentBroker:
                            Mock.Of<IRenderFileContentBroker>()),
                    new ResourceTagHandlingProcessingService(),
                    new ExecuteTagHandlingProcessingService(
                        jsonBroker: Mock.Of<IJsonBroker>(),
                        workflowExecutionBroker:
                            Mock.Of<IWorkflowExecutionBroker>())
                ])));
}