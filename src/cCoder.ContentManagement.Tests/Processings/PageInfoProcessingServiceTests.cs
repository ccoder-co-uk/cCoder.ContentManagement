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

public partial class PageInfoProcessingServiceTests
{
    private readonly Mock<IAuthorizationBroker> authorizationBrokerMock = new();
    private readonly Mock<IPageInfoService> pageInfoServiceMock = new();
    private readonly PageInfoProcessingService pageInfoProcessingService;

    public PageInfoProcessingServiceTests()
    {
        pageInfoProcessingService = new PageInfoProcessingService(pageInfoServiceMock.Object);
    }

    private static PageInfo CreateRandomPageInfo() =>
        new()
        {
            Id = Random.Shared.Next(1, 10000),
            PageId = Random.Shared.Next(1, 10000),
            CultureId = "en-GB",
            Title = $"Title-{Guid.NewGuid():N}",
            Description = "Description",
            Keywords = "Keywords",
            Page = null!,
            Culture = null!,
        };
}






















