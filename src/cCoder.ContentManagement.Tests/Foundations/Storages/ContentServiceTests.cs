// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

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

public partial class ContentServiceTests
{
    private readonly Mock<IContentBroker> contentBrokerMock;
    private readonly Mock<IPageBroker> pageBrokerMock;
    private readonly Mock<IAuthorizationBroker> authorizationBrokerMock;
    private readonly ContentService contentService;

    public ContentServiceTests()
    {
        contentBrokerMock = new Mock<IContentBroker>(behavior: MockBehavior.Strict);
        pageBrokerMock = new Mock<IPageBroker>(behavior: MockBehavior.Strict);
        authorizationBrokerMock = new Mock<IAuthorizationBroker>(behavior: MockBehavior.Strict);

        contentService = new ContentService(
contentBroker: contentBrokerMock.Object,
pageBroker: pageBrokerMock.Object,
authorizationBroker: authorizationBrokerMock.Object
        );
    }

    private static Content CreateRandomContent(int id = 42, int pageId = 7)
    {
        Content content = Builder<Content>
            .CreateNew()
            .With(func: x => x.Id = id)
            .With(func: x => x.PageId = pageId)
            .With(func: x => x.CultureId = "en-GB")
            .With(func: x => x.Name = $"Content-{Guid.NewGuid():N}")
            .With(func: x => x.Html = "<p>content</p>")
            .Build();

        return content;
    }
}