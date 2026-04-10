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
using cCoder.ContentManagement.Brokers.Storages;



using cCoder.ContentManagement.Services.Foundations.Storages;
using FizzWare.NBuilder;
using Moq;
using IAuthorizationBroker = cCoder.ContentManagement.Brokers.IAuthorizationBroker;


namespace cCoder.Core.Services.Tests.CMS.Foundations.Storages;

public partial class PageServiceTests
{
    private readonly Mock<IPageBroker> pageBrokerMock;
    private readonly Mock<IAuthorizationBroker> authorizationBrokerMock;
    private readonly PageService pageService;

    public PageServiceTests()
    {
        pageBrokerMock = new Mock<IPageBroker>(MockBehavior.Strict);
        authorizationBrokerMock = new Mock<IAuthorizationBroker>(MockBehavior.Strict);
        pageService = new PageService(pageBrokerMock.Object, authorizationBrokerMock.Object);
    }

    private static Page CreateRandomPage(int id = 42)
    {
        Page page = Builder<Page>
            .CreateNew()
            .With(x => x.Id = id)
            .With(x => x.AppId = 7)
            .With(x => x.Order = 1)
            .With(x => x.ShowOnMenus = true)
            .With(x => x.Name = $"Page-{Guid.NewGuid():N}")
            .With(x => x.LastUpdated = DateTimeOffset.UtcNow)
            .With(x => x.LastUpdatedBy = "tester")
            .With(x => x.CreatedOn = DateTimeOffset.UtcNow)
            .With(x => x.CreatedBy = "tester")
            .With(x => x.Path = $"/page-{Guid.NewGuid():N}")
            .With(x => x.ResourceKey = $"resource-{Guid.NewGuid():N}")
            .With(x => x.Layout = "Default")
            .Build();

        return page;
    }
}




















