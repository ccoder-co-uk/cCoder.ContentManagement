// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Brokers.ServiceProviders;
using cCoder.ContentManagement.Services.Foundations.Rendering;
using Moq;

namespace cCoder.ContentManagement.Tests.Foundations.Rendering;

public partial class PageRenderServiceTests
{
    private readonly Mock<IServiceProviderBroker> serviceProviderBrokerMock;
    private readonly PageRenderService pageRenderService;

    public PageRenderServiceTests()
    {
        serviceProviderBrokerMock = new Mock<IServiceProviderBroker>();

        pageRenderService =
            new PageRenderService(
                serviceProviderBroker: serviceProviderBrokerMock.Object);
    }
}