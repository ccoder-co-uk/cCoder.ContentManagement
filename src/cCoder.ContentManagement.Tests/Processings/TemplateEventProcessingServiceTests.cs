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
using cCoder.ContentManagement.Services.Foundations.Events;
using cCoder.ContentManagement.Services.Processings;
using FizzWare.NBuilder;
using Moq;


namespace cCoder.Core.Services.Tests.CMS.Processings;

public partial class TemplateEventProcessingServiceTests
{
    private readonly Mock<ITemplateEventService> templateEventServiceMock;
    private readonly TemplateEventProcessingService service;

    public TemplateEventProcessingServiceTests()
    {
        templateEventServiceMock = new Mock<ITemplateEventService>(MockBehavior.Strict);
        service = new TemplateEventProcessingService(templateEventServiceMock.Object);
    }

    private static Template CreateRandomTemplate() =>
        Builder<Template>.CreateNew().Build();
}




















