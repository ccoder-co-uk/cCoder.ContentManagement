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
using DataPageInfo = cCoder.Data.Models.CMS.PageInfo;
using IAuthorizationBroker = cCoder.ContentManagement.Brokers.IAuthorizationBroker;

namespace cCoder.Core.Services.Tests.CMS.Foundations.Storages;

public partial class PageInfoServiceTests
{
    private readonly Mock<IPageInfoBroker> pageInfoBrokerMock;
    private readonly Mock<IPageBroker> pageBrokerMock;
    private readonly Mock<IAuthorizationBroker> authorizationBrokerMock;
    private readonly PageInfoService pageInfoService;

    public PageInfoServiceTests()
    {
        pageInfoBrokerMock = new Mock<IPageInfoBroker>(behavior: MockBehavior.Strict);
        pageBrokerMock = new Mock<IPageBroker>(behavior: MockBehavior.Strict);
        authorizationBrokerMock = new Mock<IAuthorizationBroker>(behavior: MockBehavior.Strict);

        pageInfoService = new PageInfoService(
pageInfoBroker: pageInfoBrokerMock.Object,
pageBroker: pageBrokerMock.Object,
authorizationBroker: authorizationBrokerMock.Object
        );
    }

    private static PageInfo CreateRandomPageInfo(
        int id = 42,
        int pageId = 7,
        string cultureId = null
    )
    {
        PageInfo pageInfo = Builder<PageInfo>
            .CreateNew()
            .With(func: x => x.Id = id)
            .With(func: x => x.PageId = pageId)
            .With(func: x => x.CultureId = cultureId ?? "en-GB")
            .With(func: x => x.Title = $"Title-{Guid.NewGuid():N}")
            .With(func: x => x.Description = $"Description-{Guid.NewGuid():N}")
            .With(func: x => x.Keywords = $"Keywords-{Guid.NewGuid():N}")
            .Build();

        return pageInfo;
    }

    private static DataPageInfo ToDataPageInfo(PageInfo pageInfo) =>
        new()
        {
            Id = pageInfo.Id,
            PageId = pageInfo.PageId,
            CultureId = pageInfo.CultureId,
            Title = pageInfo.Title,
            Description = pageInfo.Description,
            Keywords = pageInfo.Keywords,
        };
}