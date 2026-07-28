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
using cCoder.ContentManagement.Services.Orchestrations;
using cCoder.ContentManagement.Services.Processings;
using Moq;
using DataUser = cCoder.Data.Models.Security.User;

namespace cCoder.Core.Services.Tests.CMS.Orchestrations;

public partial class ComponentRenderOrchestrationServiceTests
{
    private readonly Mock<IComponentRenderProcessingService> componentRenderProcessingServiceMock = new();
    private readonly Mock<IAuthorizationProcessingService> authorizationProcessingServiceMock = new();
    private readonly ComponentRenderOrchestrationService renderOrchestrationService;

    public ComponentRenderOrchestrationServiceTests()
    {
        renderOrchestrationService = new ComponentRenderOrchestrationService(
componentRenderProcessingService: componentRenderProcessingServiceMock.Object
,
authorizationProcessingService: authorizationProcessingServiceMock.Object
        );
    }

    private static Component CreateRandomComponent() =>
        new()
        {
            Id = Random.Shared.Next(minValue: 1, maxValue: 10000),
            AppId = 1,
            Name = $"Component-{Guid.NewGuid():N}",
            ResourceKey = "component",
            Content = "<div>content</div>",
            Script = "console.log('component');",
            Key = $"key-{Guid.NewGuid():N}",
        };

    private static DataUser CreateDataUserWithPrivilege(string privilege, int appId = 1)
    {
        cCoder.Data.Models.Security.Role role = new()
        {
            Id = Guid.NewGuid(),
            AppId = appId,
            Name = "Test Role",
            Privs = privilege.ToLowerInvariant(),
        };

        DataUser user = new()
        {
            Id = "test-user",
            DefaultCultureId = string.Empty,
            DisplayName = "Test User",
            Email = "test@example.com",
            IsActive = true,
        };

        cCoder.Data.Models.Security.UserRole userRole = new()
        {
            Role = role,
            RoleId = role.Id,
            User = user,
            UserId = user.Id,
        };

        user.Roles = [userRole];
        role.Users = [userRole];
        return user;
    }
}