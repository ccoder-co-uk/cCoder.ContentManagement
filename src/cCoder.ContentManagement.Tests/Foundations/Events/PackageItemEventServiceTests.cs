// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Brokers.Events;
using Moq;


namespace cCoder.Core.Services.Tests.CMS.Foundations.Events;

public partial class PackageItemEventServiceTests
{
    private readonly Mock<IPackageItemEventBroker> packageItemEventBrokerMock;
    private readonly cCoder.ContentManagement.Services.Foundations.Events.PackageItemEventService service;
    private const string CurrentUserId = "test-user";

    public PackageItemEventServiceTests()
    {
        packageItemEventBrokerMock = new Mock<IPackageItemEventBroker>(behavior: MockBehavior.Strict);
        packageItemEventBrokerMock = new(behavior: MockBehavior.Strict);

        service = new cCoder.ContentManagement.Services.Foundations.Events.PackageItemEventService(
packageItemEventBroker: packageItemEventBrokerMock.Object
        );
    }
}