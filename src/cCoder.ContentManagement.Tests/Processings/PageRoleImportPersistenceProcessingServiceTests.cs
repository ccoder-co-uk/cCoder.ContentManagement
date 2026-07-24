// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Brokers.Storages;
using cCoder.ContentManagement.Services.Processings;
using cCoder.Data.Models.Security;
using Moq;

namespace cCoder.Core.Services.Tests.CMS.Processings;

public partial class PageRoleImportPersistenceProcessingServiceTests
{
    private readonly Mock<IPageRoleBroker> pageRoleBrokerMock = new(
        behavior: MockBehavior.Strict);

    private readonly PageRoleImportPersistenceProcessingService processingService;

    public PageRoleImportPersistenceProcessingServiceTests()
    {
        processingService =
            new PageRoleImportPersistenceProcessingService(
                pageRoleBroker: pageRoleBrokerMock.Object);
    }

    private static PageRole CreatePageRole(
        int pageId,
        Guid roleId) =>
        new()
            {
                PageId = pageId,
                RoleId = roleId
            };
}