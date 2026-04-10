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
using cCoder.ContentManagement.Services.Foundations;
using Moq;


namespace cCoder.Core.Services.Tests.CMS.Foundations;

public partial class RenderFileContentServiceTests
{
    private readonly Mock<IRenderFileContentBroker> renderFileContentBrokerMock;
    private readonly IRenderFileContentService renderFileContentService;

    public RenderFileContentServiceTests()
    {
        renderFileContentBrokerMock = new Mock<IRenderFileContentBroker>(MockBehavior.Strict);
        renderFileContentService = new RenderFileContentService(renderFileContentBrokerMock.Object);
    }
}





