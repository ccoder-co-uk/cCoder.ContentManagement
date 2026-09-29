// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Packaging;
using cCoder.ContentManagement.Services.Foundations.Events;
using cCoder.ContentManagement.Services.Processings;
using FizzWare.NBuilder;
using Moq;



namespace cCoder.Core.Services.Tests.CMS.Processings;

public partial class PackageItemEventProcessingServiceTests
{
    private readonly Mock<IPackageItemEventService> packageItemEventServiceMock;
    private readonly PackageItemEventProcessingService service;
    private const string CurrentUserId = "test-user";

    public PackageItemEventProcessingServiceTests()
    {
        packageItemEventServiceMock = new Mock<IPackageItemEventService>(behavior: MockBehavior.Strict);
        service = new PackageItemEventProcessingService(
            eventService: packageItemEventServiceMock.Object);
    }

    private static PackageItem CreateRandomPackageItem() =>
        Builder<PackageItem>.CreateNew()
        .Build();
}