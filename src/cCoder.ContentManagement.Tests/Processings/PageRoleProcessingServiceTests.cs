// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Security;
using cCoder.ContentManagement.Services.Foundations.Storages;
using cCoder.ContentManagement.Services.Processings;
using Moq;
using LocalRole = cCoder.Data.Models.Security.Role;

namespace cCoder.Core.Services.Tests.CMS.Processings;

public partial class PageRoleProcessingServiceTests
{
    private readonly Mock<IPageRoleService> pageRoleServiceMock = new();
    private readonly PageRoleProcessingService pageRoleProcessingService;

    public PageRoleProcessingServiceTests()
    {
        pageRoleProcessingService = new PageRoleProcessingService(
service: pageRoleServiceMock.Object
        );
    }

    private static LocalRole ToLocalRole(Role role) =>
        new()
        {
            Id = role.Id,
            AppId = role.AppId,
            Name = role.Name,
            Description = role.Description,
            Privs = role.Privs,
        };
}