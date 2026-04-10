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


namespace cCoder.Core.Services.Tests.CMS.Processings;

public partial class TemplateProcessingServiceTests
{
    private readonly Mock<ITemplateService> templateServiceMock = new();
    private readonly TemplateProcessingService templateProcessingService;

    public TemplateProcessingServiceTests()
    {
        templateProcessingService = new TemplateProcessingService(templateServiceMock.Object);
    }

    private static Template CreateRandomTemplate() =>
        new()
        {
            Id = Random.Shared.Next(1, 10000),
            AppId = 1,
            Name = $"Template-{Guid.NewGuid():N}",
            ResourceKey = "template",
            RawString = "<html></html>",
        };
}






















