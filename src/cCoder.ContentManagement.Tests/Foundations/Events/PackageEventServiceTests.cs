// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Packaging;
using cCoder.Data.Models.Security;
using ComponentRenderParams = cCoder.ContentManagement.Models.ComponentRenderParams;
using Config = cCoder.ContentManagement.Models.ContentManagementConfiguration;
using PageRenderParams = cCoder.ContentManagement.Models.PageRenderParams;
using PageRoleInfo = cCoder.ContentManagement.Models.PageRoleInfo;
using RenderParams = cCoder.ContentManagement.Models.RenderParams;
using RenderResult = cCoder.ContentManagement.Models.RenderResult;
using TemplateRenderParams = cCoder.ContentManagement.Models.TemplateRenderParams;
using cCoder.ContentManagement.Brokers.Events;
using cCoder.Data;
using Moq;


namespace cCoder.Core.Services.Tests.CMS.Foundations.Events;

public partial class PackageEventServiceTests
{
    private readonly Mock<IPackageEventBroker> packageEventBrokerMock;
    private readonly cCoder.ContentManagement.Services.Foundations.Events.PackageEventService service;
    private const string CurrentUserId = "test-user";

    public PackageEventServiceTests()
    {
        packageEventBrokerMock = new Mock<IPackageEventBroker>(behavior: MockBehavior.Strict);
        packageEventBrokerMock = new(behavior: MockBehavior.Strict);

        packageEventBrokerMock.Setup(expression: x => x.GetCurrentUserId())
            .Returns(value: CurrentUserId);

        service = new cCoder.ContentManagement.Services.Foundations.Events.PackageEventService(
packageEventBroker: packageEventBrokerMock.Object
        );
    }
}