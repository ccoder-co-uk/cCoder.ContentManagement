// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System;
using cCoder.Data.Models.CMS;
using cCoder.ContentManagement.Brokers.Storages;



using cCoder.ContentManagement.Services.Foundations.Storages;
using FizzWare.NBuilder;
using Moq;

namespace cCoder.Core.Services.Tests.CMS.Foundations.Storages;

public partial class SubmissionServiceTests
{
    private readonly Mock<ISubmissionBroker> submissionBrokerMock;
    private readonly SubmissionService submissionService;

    public SubmissionServiceTests()
    {
        submissionBrokerMock = new Mock<ISubmissionBroker>(behavior: MockBehavior.Strict);

        submissionService = new SubmissionService(
submissionBroker: submissionBrokerMock.Object
        );
    }

    private static Submission CreateRandomSubmission(Guid id)
    {
        Submission submission = Builder<Submission>
            .CreateNew()
            .With(func: x => x.Id = id)
            .With(func: x => x.AppId = 7)
            .With(func: x => x.CreatedBy = "tester")
            .With(func: x => x.LastUpdatedBy = "tester")
            .With(func: x => x.CreatedOn = DateTimeOffset.UtcNow)
            .With(func: x => x.LastUpdatedOn = DateTimeOffset.UtcNow)
            .With(func: x => x.SourceComponent = $"component-{Guid.NewGuid():N}")
            .With(func: x => x.State = "New")
            .With(func: x => x.DataJson = "{}")
            .Build();

        return submission;
    }
}