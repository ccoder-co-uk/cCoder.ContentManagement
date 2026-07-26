// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Brokers;
using cCoder.ContentManagement.Brokers.Storages;
using cCoder.ContentManagement.Rendering.Brokers;
using cCoder.ContentManagement.Rendering.Services.Foundations;
using Moq;

namespace cCoder.ContentManagement.Tests.PenetrationTests;

public partial class MarkupRenderServiceTests
{
    private static MarkupRenderService CreateMarkupRenderService() =>
        new(
            componentReaderBroker: Mock.Of<IComponentReaderBroker>(),
            scriptReaderBroker: Mock.Of<IScriptReaderBroker>(),
            jsonBroker: Mock.Of<IJsonBroker>(),
            renderFileContentBroker: Mock.Of<IRenderFileContentBroker>());
}