// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Brokers.Storages;
using cCoder.ContentManagement.Services.Foundations;
using Moq;


namespace cCoder.Core.Services.Tests.CMS.Foundations;

public partial class RenderFileContentServiceTests
{
    private readonly Mock<IRenderFileContentBroker> renderFileContentBrokerMock;
    private readonly IRenderFileContentService renderFileContentService;

    public RenderFileContentServiceTests()
    {
        renderFileContentBrokerMock = new Mock<IRenderFileContentBroker>(behavior: MockBehavior.Strict);
        renderFileContentService = new RenderFileContentService(broker: renderFileContentBrokerMock.Object);
    }
}