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
using cCoder.ContentManagement.Brokers.Events;
using Moq;
using ICoreAuthInfo = cCoder.Data.ICoreAuthInfo;


namespace cCoder.Core.Services.Tests.CMS.Foundations.Events;

public partial class PageInfoEventServiceTests
{
    private readonly Mock<IPageInfoEventBroker> pageInfoEventBrokerMock;
    private readonly Mock<ICoreAuthInfo> authInfoMock;
    private readonly cCoder.ContentManagement.Services.Foundations.Events.PageInfoEventService service;
    private const string CurrentUserId = "test-user";

    public PageInfoEventServiceTests()
    {
        pageInfoEventBrokerMock = new Mock<IPageInfoEventBroker>(MockBehavior.Strict);
        authInfoMock = new Mock<ICoreAuthInfo>(MockBehavior.Strict);
        pageInfoEventBrokerMock = new(MockBehavior.Strict);
        authInfoMock = new();
        authInfoMock.SetupGet(x => x.SSOUserId).Returns(CurrentUserId);
        service = new cCoder.ContentManagement.Services.Foundations.Events.PageInfoEventService(
            pageInfoEventBrokerMock.Object,
            authInfoMock.Object
        );
    }
}














