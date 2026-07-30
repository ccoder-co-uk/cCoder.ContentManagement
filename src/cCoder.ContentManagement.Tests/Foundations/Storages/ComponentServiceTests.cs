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
using cCoder.ContentManagement.Brokers.Storages;



using cCoder.ContentManagement.Services.Foundations.Storages;
using FizzWare.NBuilder;
using Moq;
using IAuthorizationManager = cCoder.ContentManagement.Exposures.IAuthorizationManager;


using cCoder.ContentManagement.Exposures;

namespace cCoder.Core.Services.Tests.CMS.Foundations.Storages;

public partial class ComponentServiceTests
{
    private readonly Mock<IComponentBroker> componentBrokerMock;
    private readonly Mock<IAuthorizationManager> authorizationManagerMock;
    private readonly ComponentService componentService;

    public ComponentServiceTests()
    {
        componentBrokerMock = new Mock<IComponentBroker>(behavior: MockBehavior.Strict);
        authorizationManagerMock = new Mock<IAuthorizationManager>(behavior: MockBehavior.Strict);

        componentService = new ComponentService(
componentBroker: componentBrokerMock.Object,
authorizationManager: authorizationManagerMock.Object
        );
    }

    private static Component CreateRandomComponent(int id = 42, int appId = 7)
    {
        Component component = Builder<Component>
            .CreateNew()
            .With(func: x => x.Id = id)
            .With(func: x => x.AppId = appId)
            .With(func: x => x.ResourceKey = $"resource-{Guid.NewGuid():N}")
            .With(func: x => x.Content = "<div>content</div>")
            .With(func: x => x.Script = "console.log('component');")
            .With(func: x => x.Key = $"component-{Guid.NewGuid():N}")
            .With(func: x => x.Name = $"Component-{Guid.NewGuid():N}")
            .With(func: x => x.CreatedBy = "tester")
            .With(func: x => x.LastUpdatedBy = "tester")
            .With(func: x => x.CreatedOn = DateTimeOffset.UtcNow.AddMinutes(minutes: -5))
            .With(func: x => x.LastUpdated = DateTimeOffset.UtcNow)
            .Build();

        return component;
    }
}