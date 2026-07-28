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

public partial class ScriptServiceTests
{
    [Fact]
    public async Task ShouldDelegateToBrokerWhenUserIsAuthorizedForAddAsync()
    {
        // Given
        authorizationBrokerMock.Setup(expression: x => x.GetCurrentUser())
            .Returns(value: new SecurityDataModels.User { Id = "test-user" });

        Script script = CreateRandomScript(id: 7);

        CmsDataModels.Script submitted = null;


        authorizationBrokerMock.Setup(expression: x => x.Authorize(appId: (int?)7, privilege: "Script_create"));

        scriptBrokerMock
            .Setup(expression: x =>
                x.AddScriptAsync(newScript: It.Is<CmsDataModels.Script>(match: candidate => !ReferenceEquals(objA: candidate, objB: script)))
            )
            .Callback<CmsDataModels.Script>(action: candidate => submitted = candidate)
            .ReturnsAsync(valueFunction: (CmsDataModels.Script value) => value);

        // When
        Script result = await scriptService.AddScriptAsync(newScript: script);

        // Then

        result.Should()
            .BeSameAs(expected: script);

        submitted.Should()
            .NotBeNull();

        submitted.Should()
            .NotBeSameAs(unexpected: script);

        result.Should()
            .NotBeSameAs(unexpected: submitted);

        submitted
            .Should()
            .BeEquivalentTo(
expectation: script,
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
            .Excluding(expression: candidate => candidate.Id)
            );

        result
            .Should()
            .BeEquivalentTo(
expectation: script,
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
            .Excluding(expression: candidate => candidate.Id)
            );

        scriptBrokerMock.Verify(
expression: x => x.AddScriptAsync(newScript: It.Is<CmsDataModels.Script>(match: candidate => !ReferenceEquals(objA: candidate, objB: script))),
times: Times.Once
        );

        scriptBrokerMock.VerifyNoOtherCalls();
        authorizationBrokerMock.Verify(expression: x => x.Authorize(appId: (int?)7, privilege: "Script_create"), times: Times.Once);
    }

    [Fact]
    public async Task ShouldThrowSecurityExceptionWhenUserLacksCreatePrivilegeForAddAsync()
    {
        // Given
        authorizationBrokerMock.Setup(expression: x => x.GetCurrentUser())
            .Returns(value: new SecurityDataModels.User { Id = "test-user" });

        Script script = CreateRandomScript(id: 7);

        authorizationBrokerMock
            .Setup(expression: x => x.Authorize(appId: (int?)7, privilege: "Script_create"))
            .Throws(exception: new SecurityException(message: "Access Denied!"));

        // When
        Func<Task> action = async () => await scriptService.AddScriptAsync(newScript: script);

        // Then

        await action.Should()
            .ThrowAsync<SecurityException>()
            .WithMessage(expectedWildcardPattern: "Access Denied!");

        scriptBrokerMock.VerifyNoOtherCalls();
        authorizationBrokerMock.Verify(expression: x => x.Authorize(appId: (int?)7, privilege: "Script_create"), times: Times.Once);
    }

}