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
using System.Security;



using FluentAssertions;
using Moq;
using Xunit;
using CmsDataModels = cCoder.Data.Models.CMS;
using SecurityDataModels = cCoder.Data.Models.Security;


namespace cCoder.Core.Services.Tests.CMS.Foundations.Storages;

public partial class SubmissionServiceTests
{
    [Fact]
    public async Task ShouldDelegateToBrokerWhenUpdateAsync()
    {
        // Given
        Guid submissionId = new Guid(g: "11111111-1111-1111-1111-111111111111");
        Submission submission = CreateRandomSubmission(id: submissionId);

        CmsDataModels.Submission submitted = null;

        submissionBrokerMock
                    .Setup(expression: x => x.UpdateSubmissionAsync(updatedSubmission: It.IsAny<CmsDataModels.Submission>()))
                    .Callback<CmsDataModels.Submission>(action: candidate => submitted = candidate)
                    .ReturnsAsync(valueFunction: (CmsDataModels.Submission value) => value);

        // When
        Submission result = await submissionService.UpdateSubmissionAsync(updatedSubmission: submission);

        // Then

        result.Should()
            .BeSameAs(expected: submission);

        submitted.Should()
            .NotBeNull();

        submitted.Should()
            .NotBeSameAs(unexpected: submission);

        result.Should()
            .NotBeSameAs(unexpected: submitted);

        submitted
            .Should()
            .BeEquivalentTo(
expectation: submission,
config: options =>
                    options
                        .Excluding(
predicate: (FluentAssertions.Equivalency.IMemberInfo info) =>
                                info.Path.EndsWith(value: "CreatedOn")
                        )
            .Excluding(
predicate: (FluentAssertions.Equivalency.IMemberInfo info) =>
                                info.Path.EndsWith(value: "CreatedBy")
                        )
            .Excluding(
predicate: (FluentAssertions.Equivalency.IMemberInfo info) =>
                                info.Path.EndsWith(value: "LastUpdated")
                        )
            .Excluding(
predicate: (FluentAssertions.Equivalency.IMemberInfo info) =>
                                info.Path.EndsWith(value: "LastUpdatedBy")
                        )
            .Excluding(
predicate: (FluentAssertions.Equivalency.IMemberInfo info) =>
                                info.Path.EndsWith(value: "LastUpdatedOn")
                        )
            .Excluding(
predicate: (FluentAssertions.Equivalency.IMemberInfo info) =>
                                info.Path.EndsWith(value: "UpdatedBy")
                        )
            .Excluding(
predicate: (FluentAssertions.Equivalency.IMemberInfo info) =>
                                info.Path.EndsWith(value: "Created")
                        )
            );

        result
            .Should()
            .BeEquivalentTo(
expectation: submission,
config: options =>
                    options
                        .Excluding(
predicate: (FluentAssertions.Equivalency.IMemberInfo info) =>
                                info.Path.EndsWith(value: "CreatedOn")
                        )
            .Excluding(
predicate: (FluentAssertions.Equivalency.IMemberInfo info) =>
                                info.Path.EndsWith(value: "CreatedBy")
                        )
            .Excluding(
predicate: (FluentAssertions.Equivalency.IMemberInfo info) =>
                                info.Path.EndsWith(value: "LastUpdated")
                        )
            .Excluding(
predicate: (FluentAssertions.Equivalency.IMemberInfo info) =>
                                info.Path.EndsWith(value: "LastUpdatedBy")
                        )
            .Excluding(
predicate: (FluentAssertions.Equivalency.IMemberInfo info) =>
                                info.Path.EndsWith(value: "LastUpdatedOn")
                        )
            .Excluding(
predicate: (FluentAssertions.Equivalency.IMemberInfo info) =>
                                info.Path.EndsWith(value: "UpdatedBy")
                        )
            .Excluding(
predicate: (FluentAssertions.Equivalency.IMemberInfo info) =>
                                info.Path.EndsWith(value: "Created")
                        )
            );

        submissionBrokerMock.Verify(
expression: x => x.UpdateSubmissionAsync(updatedSubmission: It.IsAny<CmsDataModels.Submission>()),
times: Times.Once
        );

        submissionBrokerMock.VerifyNoOtherCalls();
    }
}