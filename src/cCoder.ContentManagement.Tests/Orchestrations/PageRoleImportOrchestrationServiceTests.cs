// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Models;
using cCoder.ContentManagement.Services.Orchestrations;
using cCoder.ContentManagement.Services.Processings;
using cCoder.Data.Models.Security;
using cCoder.Data.Models.CMS;
using Moq;

namespace cCoder.Core.Services.Tests.CMS.Orchestrations;

public partial class PageRoleImportOrchestrationServiceTests
{
    private readonly Mock<IPageRoleProcessingService>
        pageRoleProcessingServiceMock = new(
            behavior: MockBehavior.Strict);

    private readonly Mock<IPageRoleImportPersistenceProcessingService>
        persistenceProcessingServiceMock = new(
            behavior: MockBehavior.Strict);

    private readonly PageRoleImportOrchestrationService orchestrationService;

    public PageRoleImportOrchestrationServiceTests()
    {
        orchestrationService = new PageRoleImportOrchestrationService(
            pageRoleProcessingService: pageRoleProcessingServiceMock.Object,
            persistenceProcessingService:
                persistenceProcessingServiceMock.Object);
    }

    private static PageRoleInfo CreatePageRoleInfo(
        string path,
        string roleName) =>
        new()
        {
            Path = path,
            Role = roleName
        };

    private static PageRole CreatePageRole(
        int pageId,
        Guid roleId) =>
        new()
        {
            PageId = pageId,
            RoleId = roleId
        };
}