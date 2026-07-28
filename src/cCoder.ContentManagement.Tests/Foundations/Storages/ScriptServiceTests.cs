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
using IAuthorizationBroker = cCoder.ContentManagement.Brokers.IAuthorizationBroker;


namespace cCoder.Core.Services.Tests.CMS.Foundations.Storages;

public partial class ScriptServiceTests
{
    private readonly Mock<IScriptBroker> scriptBrokerMock;
    private readonly Mock<IAuthorizationBroker> authorizationBrokerMock;
    private readonly ScriptService scriptService;

    public ScriptServiceTests()
    {
        scriptBrokerMock = new Mock<IScriptBroker>(behavior: MockBehavior.Strict);
        authorizationBrokerMock = new Mock<IAuthorizationBroker>(behavior: MockBehavior.Strict);
        scriptService = new ScriptService(scriptBroker: scriptBrokerMock.Object, authorizationBroker: authorizationBrokerMock.Object);
    }

    private static Script CreateRandomScript(int id = 42, int appId = 7)
    {
        Script script = Builder<Script>
            .CreateNew()
            .With(func: x => x.Id = id)
            .With(func: x => x.AppId = appId)
            .With(func: x => x.Key = $"key-{Guid.NewGuid():N}")
            .With(func: x => x.Content = "console.log('script');")
            .With(func: x => x.Name = $"Script-{Guid.NewGuid():N}")
            .With(func: x => x.CreatedBy = "tester")
            .With(func: x => x.LastUpdatedBy = "tester")
            .With(func: x => x.CreatedOn = DateTimeOffset.UtcNow.AddMinutes(minutes: -5))
            .With(func: x => x.LastUpdated = DateTimeOffset.UtcNow)
            .Build();

        return script;
    }
}