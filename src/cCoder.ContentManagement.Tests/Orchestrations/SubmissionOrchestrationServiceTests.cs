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
using cCoder.ContentManagement.Services.Orchestrations;
using cCoder.ContentManagement.Services.Processings;
using FizzWare.NBuilder;
using Moq;


namespace cCoder.Core.Services.Tests.CMS.Orchestrations;

public partial class SubmissionOrchestrationServiceTests
{
    private readonly Mock<ISubmissionProcessingService> submissionProcessingServiceMock;
    private readonly Mock<ISubmissionEventProcessingService> submissionEventProcessingServiceMock;
    private readonly SubmissionOrchestrationService orchestrationService;

    public SubmissionOrchestrationServiceTests()
    {
        submissionProcessingServiceMock = new Mock<ISubmissionProcessingService>(MockBehavior.Strict);
        submissionEventProcessingServiceMock = new Mock<ISubmissionEventProcessingService>(MockBehavior.Strict);
        orchestrationService = new SubmissionOrchestrationService(
            submissionProcessingServiceMock.Object,
            submissionEventProcessingServiceMock.Object
        );
    }

    private static Submission CreateRandomSubmission() => Builder<Submission>.CreateNew().Build();
}






















