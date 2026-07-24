// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Brokers.ServiceProviders;
using cCoder.ContentManagement.Services.Foundations.Rendering;
using Moq;

namespace cCoder.ContentManagement.Tests.Foundations.Rendering;

public partial class ComponentRenderServiceTests
{
    private readonly Mock<IServiceProviderBroker> serviceProviderBrokerMock;
    private readonly ComponentRenderService componentRenderService;

    public ComponentRenderServiceTests()
    {
        serviceProviderBrokerMock = new Mock<IServiceProviderBroker>();

        componentRenderService =
            new ComponentRenderService(
                serviceProviderBroker: serviceProviderBrokerMock.Object);
    }
}