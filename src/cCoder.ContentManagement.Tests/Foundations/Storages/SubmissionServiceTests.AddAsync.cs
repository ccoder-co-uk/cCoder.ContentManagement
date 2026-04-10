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
    public async Task ShouldDelegateToBrokerWhenUserIsAuthorizedForAddAsync()
    {
        // Given
        authorizationBrokerMock.Setup(x => x.GetCurrentUser()).Returns(new SecurityDataModels.User { Id = "test-user" });
        Submission submission = CreateRandomSubmission(Guid.Empty);

        CmsDataModels.Submission submitted = null;


        authorizationBrokerMock.Setup(x => x.Authorize((int?)7, "Submission_create"));

        submissionBrokerMock
            .Setup(x =>
                x.AddSubmissionAsync(
                    It.Is<CmsDataModels.Submission>(candidate => !ReferenceEquals(candidate, submission))
                )
            )
            .Callback<CmsDataModels.Submission>(candidate => submitted = candidate)
            .ReturnsAsync((CmsDataModels.Submission value) => value);

        // When
        Submission result = await submissionService.AddAsync(submission);

        // Then
        result.Should().NotBeSameAs(submission);
        submitted.Should().NotBeNull();

        submitted
            .Should()
            .BeEquivalentTo(
                submission,
                options =>
                    options
                        .Excluding(
                            (FluentAssertions.Equivalency.IMemberInfo info) =>
                                info.Path.EndsWith("CreatedOn")
                        )
                        .Excluding(
                            (FluentAssertions.Equivalency.IMemberInfo info) =>
                                info.Path.EndsWith("CreatedBy")
                        )
                        .Excluding(
                            (FluentAssertions.Equivalency.IMemberInfo info) =>
                                info.Path.EndsWith("LastUpdated")
                        )
                        .Excluding(
                            (FluentAssertions.Equivalency.IMemberInfo info) =>
                                info.Path.EndsWith("LastUpdatedBy")
                        )
                        .Excluding(
                            (FluentAssertions.Equivalency.IMemberInfo info) =>
                                info.Path.EndsWith("LastUpdatedOn")
                        )
                        .Excluding(
                            (FluentAssertions.Equivalency.IMemberInfo info) =>
                                info.Path.EndsWith("UpdatedBy")
                        )
                        .Excluding(
                            (FluentAssertions.Equivalency.IMemberInfo info) =>
                                info.Path.EndsWith("Created")
                        )
                        .Excluding(candidate => candidate.Id)
            );

        result
            .Should()
            .BeEquivalentTo(
                submission,
                options =>
                    options
                        .Excluding(
                            (FluentAssertions.Equivalency.IMemberInfo info) =>
                                info.Path.EndsWith("CreatedOn")
                        )
                        .Excluding(
                            (FluentAssertions.Equivalency.IMemberInfo info) =>
                                info.Path.EndsWith("CreatedBy")
                        )
                        .Excluding(
                            (FluentAssertions.Equivalency.IMemberInfo info) =>
                                info.Path.EndsWith("LastUpdated")
                        )
                        .Excluding(
                            (FluentAssertions.Equivalency.IMemberInfo info) =>
                                info.Path.EndsWith("LastUpdatedBy")
                        )
                        .Excluding(
                            (FluentAssertions.Equivalency.IMemberInfo info) =>
                                info.Path.EndsWith("LastUpdatedOn")
                        )
                        .Excluding(
                            (FluentAssertions.Equivalency.IMemberInfo info) =>
                                info.Path.EndsWith("UpdatedBy")
                        )
                        .Excluding(
                            (FluentAssertions.Equivalency.IMemberInfo info) =>
                                info.Path.EndsWith("Created")
                        )
                        .Excluding(candidate => candidate.Id)
            );

        submissionBrokerMock.Verify(
            x =>
                x.AddSubmissionAsync(
                    It.Is<CmsDataModels.Submission>(candidate => !ReferenceEquals(candidate, submission))
                ),
            Times.Once
        );
        submissionBrokerMock.VerifyNoOtherCalls();
        authorizationBrokerMock.Verify(x => x.Authorize((int?)7, "Submission_create"), Times.Once);
    }

    [Fact]
    public async Task ShouldThrowSecurityExceptionWhenUserLacksCreatePrivilegeForAddAsync()
    {
        // Given
        authorizationBrokerMock.Setup(x => x.GetCurrentUser()).Returns(new SecurityDataModels.User { Id = "test-user" });
        Submission submission = CreateRandomSubmission(Guid.Empty);

        authorizationBrokerMock
            .Setup(x => x.Authorize((int?)7, "Submission_create"))
            .Throws(new SecurityException("Access Denied!"));

        // When
        Func<Task> action = async () => await submissionService.AddAsync(submission);

        // Then
        await action.Should().ThrowAsync<SecurityException>().WithMessage("Access Denied!");
        submissionBrokerMock.VerifyNoOtherCalls();
        authorizationBrokerMock.Verify(x => x.Authorize((int?)7, "Submission_create"), Times.Once);
    }

}















