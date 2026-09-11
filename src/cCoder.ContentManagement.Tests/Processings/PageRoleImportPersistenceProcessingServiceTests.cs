// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Services.Foundations.Storages;
using cCoder.ContentManagement.Services.Processings;
using cCoder.Data.Models.Security;
using Moq;

namespace cCoder.Core.Services.Tests.CMS.Processings;

public partial class PageRoleImportPersistenceProcessingServiceTests
{
    private readonly Mock<IPageRoleService> pageRoleServiceMock = new(
        behavior: MockBehavior.Strict);

    private Mock<IPageRoleService> pageRoleBrokerMock => pageRoleServiceMock;

    private readonly PageRoleImportPersistenceProcessingService processingService;

    public PageRoleImportPersistenceProcessingServiceTests()
    {
        processingService =
            new PageRoleImportPersistenceProcessingService(
                service: pageRoleServiceMock.Object);
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