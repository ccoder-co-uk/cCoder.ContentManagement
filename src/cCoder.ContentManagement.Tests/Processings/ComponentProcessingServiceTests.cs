using cCoder.Data.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Packaging;
using cCoder.Data.Models.Security;
using ComponentRenderParams = cCoder.ContentManagement.Models.ComponentRenderParams;
using Config = cCoder.ContentManagement.Models.Config;
using PageRenderParams = cCoder.ContentManagement.Models.PageRenderParams;
using PageRoleInfo = cCoder.ContentManagement.Models.PageRoleInfo;
using RenderParams = cCoder.ContentManagement.Models.RenderParams;
using RenderResult = cCoder.ContentManagement.Models.RenderResult;
using TemplateRenderParams = cCoder.ContentManagement.Models.TemplateRenderParams;
using cCoder.ContentManagement.Services.Foundations.Storages;
using cCoder.ContentManagement.Services.Processings;
using Moq;


namespace cCoder.Core.Services.Tests.CMS.Processings;

public partial class ComponentProcessingServiceTests
{
    private readonly Mock<IComponentService> componentServiceMock = new();
    private readonly ComponentProcessingService componentProcessingService;

    public ComponentProcessingServiceTests()
    {
        componentProcessingService = new ComponentProcessingService(componentServiceMock.Object);
    }

    private static Component CreateRandomComponent() =>
        new()
        {
            Id = Random.Shared.Next(1, 10000),
            AppId = 1,
            Name = $"Component-{Guid.NewGuid():N}",
            ResourceKey = "component",
            Content = "<div>content</div>",
            Script = "console.log('component');",
            Key = $"key-{Guid.NewGuid():N}",
        };

}






















