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

public partial class LayoutEventProcessingServiceTests
{
    private readonly Mock<ILayoutEventService> layoutEventServiceMock;
    private readonly LayoutEventProcessingService service;

    public LayoutEventProcessingServiceTests()
    {
        layoutEventServiceMock = new Mock<ILayoutEventService>(MockBehavior.Strict);
        service = new LayoutEventProcessingService(layoutEventServiceMock.Object);
    }

    private static Layout CreateRandomLayout() =>
        Builder<Layout>.CreateNew().Build();
}




















