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
using FizzWare.NBuilder;
using Moq;
using IAuthorizationBroker = cCoder.ContentManagement.Brokers.IAuthorizationBroker;


namespace cCoder.Core.Services.Tests.CMS.Processings;

public partial class PageProcessingServiceTests
{
    private User currentUser = TestUsers.WithoutPrivileges();
    private readonly Mock<IPageService> pageServiceMock = new();
    private readonly Mock<IAuthorizationBroker> authorizationBrokerMock = new();
    private readonly PageProcessingService pageProcessingService;

    public PageProcessingServiceTests()
    {
        pageProcessingService = new PageProcessingService(
            pageServiceMock.Object,
            authorizationBrokerMock.Object
        );
    }

    private static Page CreateRandomPage(User user = null)
    {
        Page page = Builder<Page>
            .CreateNew()
            .With(x => x.Id = Random.Shared.Next(1, 10000))
            .With(x => x.AppId = 1)
            .With(x => x.Name = $"Page-{Guid.NewGuid():N}")
            .With(x => x.Path = string.Empty)
            .With(x => x.ParentId = null)
            .With(x => x.App = null)
            .With(x => x.Parent = null)
            .With(x => x.Pages = [])
            .With(x => x.Contents = [])
            .With(x => x.Roles = [])
            .Build();

        page.PageInfo =
        [
            Builder<PageInfo>
                .CreateNew()
                .With(x => x.Id = Random.Shared.Next(1, 10000))
                .With(x => x.PageId = page.Id)
                .With(x => x.CultureId = string.Empty)
                .With(x => x.Title = $"Title-{Guid.NewGuid():N}")
                                .With(x => x.Culture = null)
                .Build(),
        ];

        page.Roles =
            user == null
                ? []
                :
                [
                    new PageRole
                    {
                        RoleId = user.Roles.First().RoleId,
                        Role = user.Roles.First().Role,
                        Page = null!,
                    },
                ];

        return page;
    }
}
























