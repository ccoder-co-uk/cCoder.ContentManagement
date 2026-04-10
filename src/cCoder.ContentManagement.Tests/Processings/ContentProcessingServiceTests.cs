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
using FizzWare.NBuilder;
using Moq;


namespace cCoder.Core.Services.Tests.CMS.Processings;

public partial class ContentProcessingServiceTests
{
    private User currentUser = TestUsers.WithoutPrivileges();
    private readonly Mock<IContentService> contentServiceMock = new();
    private readonly ContentProcessingService contentProcessingService;

    public ContentProcessingServiceTests()
    {
        contentProcessingService = new ContentProcessingService(contentServiceMock.Object);
    }

    private static Content CreateRandomContent() =>
        Builder<Content>
            .CreateNew()
            .With(x => x.Id = Random.Shared.Next(1, 10000))
            .With(x => x.PageId = Random.Shared.Next(1, 10000))
            .With(x => x.CultureId = string.Empty)
            .With(x => x.Name = $"Content-{Guid.NewGuid():N}")
            .With(x => x.Html = $"<p>{Guid.NewGuid():N}</p>")
            .Build();
}


















