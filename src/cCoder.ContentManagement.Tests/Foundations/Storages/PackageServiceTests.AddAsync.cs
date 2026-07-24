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



namespace cCoder.Core.Services.Tests.CMS.Foundations.Storages;

public partial class PackageServiceTests
{
    [Fact]
    public async Task ShouldDelegateToBrokerWhenUserIsAuthorizedForAddAsync()
    {
        // Given
        Package package = CreateRandomPackage();

        cCoder.Data.Models.Packaging.Package submitted = null;

        authorizationBrokerMock.Setup(expression: x => x.Authorize(appId: null, privilege: "Package_create"));

        packageBrokerMock
            .Setup(expression: x =>
                x.AddPackageAsync(newPackage: It.Is<cCoder.Data.Models.Packaging.Package>(match: candidate => !ReferenceEquals(objA: candidate, objB: package)))
            )
            .Callback<cCoder.Data.Models.Packaging.Package>(action: candidate => submitted = candidate)
            .ReturnsAsync(valueFunction: (cCoder.Data.Models.Packaging.Package value) => value);

        // When
        Package result = await packageService.AddPackageAsync(newPackage: package);

        // Then

        result.Should()
            .BeSameAs(expected: package);

        submitted.Should()
            .NotBeNull();

        submitted.Should()
            .NotBeSameAs(unexpected: package);

        result.Should()
            .NotBeSameAs(unexpected: submitted);

        submitted
            .Should()
            .BeEquivalentTo(
expectation: new
{
    package.Id,
    package.Name,
    package.Description,
    package.Category,
    package.SourceApi
});

        submitted.Items.Should()
            .BeNull();

        result
            .Should()
            .BeEquivalentTo(
expectation: new
{
    package.Id,
    package.Name,
    package.Description,
    package.Category,
    package.SourceApi
});

        result.Items.Should()
            .BeEquivalentTo(expectation: package.Items);

        packageBrokerMock.Verify(
expression: x =>
                x.AddPackageAsync(
newPackage: It.Is<cCoder.Data.Models.Packaging.Package>(match: candidate => !ReferenceEquals(objA: candidate, objB: package))
                ),
times: Times.Once
        );

        packageBrokerMock.VerifyNoOtherCalls();
        authorizationBrokerMock.Verify(expression: x => x.Authorize(appId: null, privilege: "Package_create"), times: Times.Once);
        authorizationBrokerMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ShouldThrowSecurityExceptionWhenUserLacksCreatePrivilegeForAddAsync()
    {
        // Given
        Package package = CreateRandomPackage();

        authorizationBrokerMock
            .Setup(expression: x => x.Authorize(appId: null, privilege: "Package_create"))
            .Throws(exception: new SecurityException(message: "Access Denied!"));

        // When
        Func<Task> action = async () => await packageService.AddPackageAsync(newPackage: package);

        // Then

        await action.Should()
            .ThrowAsync<SecurityException>()
            .WithMessage(expectedWildcardPattern: "Access Denied!");

        packageBrokerMock.VerifyNoOtherCalls();
        authorizationBrokerMock.Verify(expression: x => x.Authorize(appId: null, privilege: "Package_create"), times: Times.Once);
        authorizationBrokerMock.VerifyNoOtherCalls();
    }

}