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
        submissionProcessingService = new SubmissionProcessingService(service: submissionServiceMock.Object);
    }

    private static Submission CreateRandomSubmission() =>
        Builder<Submission>
            .CreateNew()
        .With(func: x => x.Id = Guid.NewGuid())
        .With(func: x => x.AppId = 1)
        .With(func: x => x.CreatedBy = "test-user")
        .With(func: x => x.LastUpdatedBy = "test-user")
        .With(func: x => x.CreatedOn = DateTimeOffset.UtcNow)
        .With(func: x => x.LastUpdatedOn = DateTimeOffset.UtcNow)
        .With(func: x => x.SourceComponent = $"Component-{Guid.NewGuid():N}")
        .With(func: x => x.State = "New")
        .With(func: x => x.DataJson = "{}")
        .With(func: x => x.App = null)
        .Build();
}