// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Brokers.ServiceProviders;
using cCoder.ContentManagement.Services.Foundations.ServiceProviders;
using Moq;

namespace cCoder.ContentManagement.Tests.Foundations.ServiceProviders;

public partial class ServiceProviderExecutionServiceTests
{
    private readonly Mock<IServiceProviderBroker> serviceProviderBrokerMock;
    private readonly ServiceProviderExecutionService serviceProviderExecutionService;

    public ServiceProviderExecutionServiceTests()
    {
        serviceProviderBrokerMock = new Mock<IServiceProviderBroker>();

        serviceProviderExecutionService =
            new ServiceProviderExecutionService(
                serviceProviderBroker: serviceProviderBrokerMock.Object);
    }
}
