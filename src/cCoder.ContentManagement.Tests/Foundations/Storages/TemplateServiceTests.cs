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

public partial class TemplateServiceTests
{
    private readonly Mock<ITemplateBroker> templateBrokerMock;
    private readonly Mock<IAuthorizationBroker> authorizationBrokerMock;
    private readonly TemplateService templateService;

    public TemplateServiceTests()
    {
        templateBrokerMock = new Mock<ITemplateBroker>(behavior: MockBehavior.Strict);
        authorizationBrokerMock = new Mock<IAuthorizationBroker>(behavior: MockBehavior.Strict);

        templateService = new TemplateService(
templateBroker: templateBrokerMock.Object,
authorizationBroker: authorizationBrokerMock.Object
        );
    }

    private static Template CreateRandomTemplate(int id = 42)
    {
        Template template = Builder<Template>
            .CreateNew()
            .With(func: x => x.Id = id)
            .With(func: x => x.Name = $"Template-{Guid.NewGuid():N}")
            .With(func: x => x.ResourceKey = $"resource-{Guid.NewGuid():N}")
            .With(func: x => x.RawString = "<html></html>")
            .With(func: x => x.AppId = 7)
            .With(func: x => x.LastUpdated = DateTimeOffset.UtcNow)
            .Build();

        return template;
    }
}