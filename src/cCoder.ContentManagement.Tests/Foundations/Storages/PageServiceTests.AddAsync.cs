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

public partial class PageServiceTests
{
    [Fact]
    public async Task ShouldDelegateToBrokerWhenUserIsAuthorizedForAddAsync()
    {
        // Given
        authorizationBrokerMock.Setup(expression: x => x.GetCurrentUser())
            .Returns(value: new SecurityDataModels.User { Id = "test-user" });

        Page page = CreateRandomPage(id: 0);

        CmsDataModels.Page submitted = null;


        authorizationBrokerMock.Setup(expression: x => x.Authorize(appId: (int?)7, privilege: "Page_create"));

        pageBrokerMock
            .Setup(expression: x => x.AddPageAsync(newPage: It.Is<CmsDataModels.Page>(match: candidate => !ReferenceEquals(objA: candidate, objB: page))))
            .Callback<CmsDataModels.Page>(action: candidate => submitted = candidate)
            .ReturnsAsync(valueFunction: (CmsDataModels.Page value) => value);

        // When
        Page result = await pageService.AddPageAsync(newPage: page);

        // Then

        result.Should()
            .BeSameAs(expected: page);

        submitted.Should()
            .NotBeNull();

        submitted.Should()
            .NotBeSameAs(unexpected: page);

        result.Should()
            .NotBeSameAs(unexpected: submitted);

        submitted
            .Should()
            .BeEquivalentTo(
expectation: page,
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
expectation: page,
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

        pageBrokerMock.Verify(
expression: x => x.AddPageAsync(newPage: It.Is<CmsDataModels.Page>(match: candidate => !ReferenceEquals(objA: candidate, objB: page))),
times: Times.Once
        );

        pageBrokerMock.VerifyNoOtherCalls();
        authorizationBrokerMock.Verify(expression: x => x.Authorize(appId: (int?)7, privilege: "Page_create"), times: Times.Once);
    }

    [Fact]
    public async Task ShouldPreserveShowOnMenusWhenAddingHiddenPageAsync()
    {
        // Given
        authorizationBrokerMock.Setup(expression: x => x.GetCurrentUser())
            .Returns(value: new SecurityDataModels.User { Id = "test-user" });

        Page page = CreateRandomPage(id: 0);
        page.ShowOnMenus = false;

        CmsDataModels.Page submitted = null;

        authorizationBrokerMock.Setup(expression: x => x.Authorize(appId: (int?)7, privilege: "Page_create"));

        pageBrokerMock
            .Setup(expression: x => x.AddPageAsync(newPage: It.Is<CmsDataModels.Page>(match: candidate => !ReferenceEquals(objA: candidate, objB: page))))
            .Callback<CmsDataModels.Page>(action: candidate => submitted = candidate)
            .ReturnsAsync(valueFunction: (CmsDataModels.Page value) => value);

        // When
        Page result = await pageService.AddPageAsync(newPage: page);

        // Then

        submitted.Should()
            .NotBeNull();

        submitted.ShowOnMenus.Should()
            .BeFalse();

        result.ShowOnMenus.Should()
            .BeFalse();

        pageBrokerMock.Verify(
expression: x => x.AddPageAsync(newPage: It.Is<CmsDataModels.Page>(match: candidate => !ReferenceEquals(objA: candidate, objB: page))),
times: Times.Once);

        pageBrokerMock.VerifyNoOtherCalls();
        authorizationBrokerMock.Verify(expression: x => x.Authorize(appId: (int?)7, privilege: "Page_create"), times: Times.Once);
    }

    [Fact]
    public async Task ShouldThrowSecurityExceptionWhenUserLacksCreatePrivilegeForAddAsync()
    {
        // Given
        authorizationBrokerMock.Setup(expression: x => x.GetCurrentUser())
            .Returns(value: new SecurityDataModels.User { Id = "test-user" });

        Page page = CreateRandomPage(id: 0);

        authorizationBrokerMock
            .Setup(expression: x => x.Authorize(appId: (int?)7, privilege: "Page_create"))
            .Throws(exception: new SecurityException(message: "Access Denied!"));

        // When
        Func<Task> action = async () => await pageService.AddPageAsync(newPage: page);

        // Then

        await action.Should()
            .ThrowAsync<SecurityException>()
            .WithMessage(expectedWildcardPattern: "Access Denied!");

        pageBrokerMock.VerifyNoOtherCalls();
        authorizationBrokerMock.Verify(expression: x => x.Authorize(appId: (int?)7, privilege: "Page_create"), times: Times.Once);
    }

}