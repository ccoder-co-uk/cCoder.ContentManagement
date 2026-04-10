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

public partial class SubmissionProcessingServiceTests
{
    private User currentUser = TestUsers.WithoutPrivileges();
    private readonly Mock<ISubmissionService> submissionServiceMock = new();
    private readonly SubmissionProcessingService submissionProcessingService;

    public SubmissionProcessingServiceTests()
    {
        submissionProcessingService = new SubmissionProcessingService(submissionServiceMock.Object);
    }

    private static Submission CreateRandomSubmission() =>
        Builder<Submission>
            .CreateNew()
            .With(x => x.Id = Guid.NewGuid())
            .With(x => x.AppId = 1)
            .With(x => x.CreatedBy = "test-user")
            .With(x => x.LastUpdatedBy = "test-user")
            .With(x => x.CreatedOn = DateTimeOffset.UtcNow)
            .With(x => x.LastUpdatedOn = DateTimeOffset.UtcNow)
            .With(x => x.SourceComponent = $"Component-{Guid.NewGuid():N}")
            .With(x => x.State = "New")
            .With(x => x.DataJson = "{}")
            .With(x => x.App = null)
            .Build();
}


















