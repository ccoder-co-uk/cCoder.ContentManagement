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
using IAuthorizationBroker = cCoder.ContentManagement.Brokers.IAuthorizationBroker;


namespace cCoder.Core.Services.Tests.CMS.Processings;

public partial class ResourceProcessingServiceTests
{
    private readonly Mock<IResourceService> resourceServiceMock = new();
    private readonly Mock<IAuthorizationBroker> authorizationBrokerMock = new();
    private readonly ResourceProcessingService resourceProcessingService;

    public ResourceProcessingServiceTests()
    {
        authorizationBrokerMock.Setup(x => x.GetCurrentUser())
            .Returns(() => TestUsers.WithoutPrivileges());

        resourceProcessingService = new ResourceProcessingService(
            resourceServiceMock.Object,
            authorizationBrokerMock.Object
        );
    }

    private static Resource CreateRandomResource(
        int id = 1,
        int appId = 1,
        string culture = "",
        string key = "key"
    ) =>
        new()
        {
            Id = id,
            AppId = appId,
            Name = $"Resource-{Guid.NewGuid():N}",
            Key = key,
            Culture = culture,
            DisplayName = "Display",
            ShortDisplayName = "Display",
            Description = "Description",
        };
}




















