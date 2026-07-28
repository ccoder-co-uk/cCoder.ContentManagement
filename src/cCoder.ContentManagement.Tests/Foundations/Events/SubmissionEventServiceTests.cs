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
using cCoder.ContentManagement.Brokers.Events;
using cCoder.Data;
using Moq;


namespace cCoder.Core.Services.Tests.CMS.Foundations.Events;

public partial class SubmissionEventServiceTests
{
    private readonly Mock<ISubmissionEventBroker> submissionEventBrokerMock;
    private readonly cCoder.ContentManagement.Services.Foundations.Events.SubmissionEventService service;
    private const string CurrentUserId = "test-user";

    public SubmissionEventServiceTests()
    {
        submissionEventBrokerMock = new Mock<ISubmissionEventBroker>(behavior: MockBehavior.Strict);
        submissionEventBrokerMock = new(behavior: MockBehavior.Strict);

        submissionEventBrokerMock.Setup(expression: x => x.GetCurrentUserId())
            .Returns(value: CurrentUserId);

        service = new cCoder.ContentManagement.Services.Foundations.Events.SubmissionEventService(
submissionEventBroker: submissionEventBrokerMock.Object
        );
    }
}