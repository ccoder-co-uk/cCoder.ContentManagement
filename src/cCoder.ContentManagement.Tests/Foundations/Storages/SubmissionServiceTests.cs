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

public partial class SubmissionServiceTests
{
    private readonly Mock<ISubmissionBroker> submissionBrokerMock;
    private readonly Mock<IAuthorizationBroker> authorizationBrokerMock;
    private readonly SubmissionService submissionService;

    public SubmissionServiceTests()
    {
        submissionBrokerMock = new Mock<ISubmissionBroker>(MockBehavior.Strict);
        authorizationBrokerMock = new Mock<IAuthorizationBroker>(MockBehavior.Strict);
        submissionService = new SubmissionService(
            submissionBrokerMock.Object,
            authorizationBrokerMock.Object
        );
    }

    private static Submission CreateRandomSubmission(Guid id)
    {
        Submission submission = Builder<Submission>
            .CreateNew()
            .With(x => x.Id = id)
            .With(x => x.AppId = 7)
            .With(x => x.CreatedBy = "tester")
            .With(x => x.LastUpdatedBy = "tester")
            .With(x => x.CreatedOn = DateTimeOffset.UtcNow)
            .With(x => x.LastUpdatedOn = DateTimeOffset.UtcNow)
            .With(x => x.SourceComponent = $"component-{Guid.NewGuid():N}")
            .With(x => x.State = "New")
            .With(x => x.DataJson = "{}")
            .Build();

        return submission;
    }
}




















