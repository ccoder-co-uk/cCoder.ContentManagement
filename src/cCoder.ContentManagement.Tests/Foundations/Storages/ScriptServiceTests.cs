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

public partial class ScriptServiceTests
{
    private readonly Mock<IScriptBroker> scriptBrokerMock;
    private readonly Mock<IAuthorizationBroker> authorizationBrokerMock;
    private readonly ScriptService scriptService;

    public ScriptServiceTests()
    {
        scriptBrokerMock = new Mock<IScriptBroker>(MockBehavior.Strict);
        authorizationBrokerMock = new Mock<IAuthorizationBroker>(MockBehavior.Strict);
        scriptService = new ScriptService(scriptBrokerMock.Object, authorizationBrokerMock.Object);
    }

    private static Script CreateRandomScript(int id = 42, int appId = 7)
    {
        Script script = Builder<Script>
            .CreateNew()
            .With(x => x.Id = id)
            .With(x => x.AppId = appId)
            .With(x => x.Key = $"key-{Guid.NewGuid():N}")
            .With(x => x.Content = "console.log('script');")
            .With(x => x.Name = $"Script-{Guid.NewGuid():N}")
            .With(x => x.CreatedBy = "tester")
            .With(x => x.LastUpdatedBy = "tester")
            .With(x => x.CreatedOn = DateTimeOffset.UtcNow.AddMinutes(-5))
            .With(x => x.LastUpdated = DateTimeOffset.UtcNow)
            .Build();

        return script;
    }
}




















