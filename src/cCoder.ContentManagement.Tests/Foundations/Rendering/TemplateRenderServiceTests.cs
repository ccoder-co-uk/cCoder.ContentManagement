// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Brokers.ServiceProviders;
using cCoder.ContentManagement.Services.Foundations.Rendering;
using Moq;

namespace cCoder.ContentManagement.Tests.Foundations.Rendering;

public partial class TemplateRenderServiceTests
{
    private readonly Mock<IServiceProviderBroker> serviceProviderBrokerMock;
    private readonly TemplateRenderService templateRenderService;

    public TemplateRenderServiceTests()
    {
        serviceProviderBrokerMock = new Mock<IServiceProviderBroker>();

        templateRenderService =
            new TemplateRenderService(
                serviceProviderBroker: serviceProviderBrokerMock.Object);
    }
}