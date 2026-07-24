// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

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

public partial class ResourceServiceTests
{
    [Fact]
    public async Task ShouldDelegateToDataContextWhenUserIsAuthorizedForAddAsync()
    {
        // Given

        authorizationBrokerMock
            .Setup(expression: x => x.GetCurrentUser())
            .Returns(value: new SecurityDataModels.User { Id = "test-user" });

        Resource resource = CreateRandomResource(id: 0, appId: 7);
        CmsDataModels.Resource submitted = null;
        authorizationBrokerMock.Setup(expression: x => x.Authorize(appId: (int?)7, privilege: "Resource_create"));

        resourceBrokerMock
            .Setup(expression: x =>
                x.AddResourceAsync(
newResource: It.Is<CmsDataModels.Resource>(match: candidate => !ReferenceEquals(objA: candidate, objB: resource))
                )
            )
            .Callback<CmsDataModels.Resource>(action: candidate => submitted = candidate)
            .ReturnsAsync(valueFunction: (CmsDataModels.Resource value) => value);

        // When
        Resource result = await resourceService.AddResourceAsync(resource: resource);

        // Then

        result.Should()
            .BeSameAs(expected: resource);

        submitted.Should()
            .NotBeNull();

        submitted.Should()
            .NotBeSameAs(unexpected: resource);

        result.Should()
            .NotBeSameAs(unexpected: submitted);

        submitted
            .Should()
            .BeEquivalentTo(
expectation: resource,
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
expectation: resource,
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

        resourceBrokerMock.Verify(
expression: x =>
                x.AddResourceAsync(
newResource: It.Is<CmsDataModels.Resource>(match: candidate => !ReferenceEquals(objA: candidate, objB: resource))
                ),
times: Times.Once
        );
    }

    [Fact]
    public async Task ShouldThrowSecurityExceptionWhenUserLacksCreatePrivilegeForAddAsync()
    {
        // Given

        authorizationBrokerMock
            .Setup(expression: x => x.GetCurrentUser())
            .Returns(value: new SecurityDataModels.User { Id = "test-user" });

        Resource resource = CreateRandomResource(id: 0, appId: 7);

        authorizationBrokerMock
            .Setup(expression: x => x.Authorize(appId: (int?)7, privilege: "Resource_create"))
            .Throws(exception: new SecurityException(message: "Access Denied!"));

        // When

        await Assert.ThrowsAsync<cCoder.ContentManagement.Models.Exceptions.ContentManagementSecurityException>(testCode: async () =>
            await resourceService.AddResourceAsync(resource: resource)
        );

        // Then
    }

}