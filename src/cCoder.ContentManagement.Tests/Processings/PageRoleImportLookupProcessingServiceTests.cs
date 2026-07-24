// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Brokers;
using cCoder.ContentManagement.Brokers.Storages;
using cCoder.ContentManagement.Services.Processings;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Security;
using Moq;

namespace cCoder.Core.Services.Tests.CMS.Processings;

public partial class PageRoleImportLookupProcessingServiceTests
{
    private readonly Mock<IRoleBroker> roleBrokerMock = new(
        behavior: MockBehavior.Strict);

    private readonly Mock<IPageBroker> pageBrokerMock = new(
        behavior: MockBehavior.Strict);

    private readonly PageRoleImportLookupProcessingService processingService;

    public PageRoleImportLookupProcessingServiceTests()
    {
        processingService = new PageRoleImportLookupProcessingService(
            roleBroker: roleBrokerMock.Object,
            pageBroker: pageBrokerMock.Object);
    }

    private static Page CreatePage(int appId, string path) =>
        new()
        {
            Id = 123,
            AppId = appId,
            Path = path
        };

    private static Role CreateRole(int appId, string roleName) =>
        new()
        {
            Id = Guid.NewGuid(),
            AppId = appId,
            Name = roleName
        };
}