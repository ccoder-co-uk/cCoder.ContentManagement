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
using FluentAssertions;
using Moq;
using Xunit;



namespace cCoder.Core.Services.Tests.CMS.Foundations.Storages;

public partial class PackageItemServiceTests
{
    [Fact]
    public async Task ShouldDelegateToBrokerWhenUpdateAsync()
    {
        // Given
        PackageItem packageItem = CreateRandomPackageItem();

        cCoder.Data.Models.Packaging.PackageItem submitted = null;

        packageItemBrokerMock
            .Setup(expression: x => x.UpdatePackageItemAsync(updatedPackageItem: It.IsAny<cCoder.Data.Models.Packaging.PackageItem>()))
            .Callback<cCoder.Data.Models.Packaging.PackageItem>(action: candidate => submitted = candidate)
            .ReturnsAsync(valueFunction: (cCoder.Data.Models.Packaging.PackageItem value) => value);

        // When
        PackageItem result = await packageItemService.UpdatePackageItemAsync(updatedPackageItem: packageItem);

        // Then

        result.Should()
            .BeSameAs(expected: packageItem);

        submitted.Should()
            .NotBeNull();

        submitted.Should()
            .NotBeSameAs(unexpected: packageItem);

        result.Should()
            .NotBeSameAs(unexpected: submitted);

        submitted.Should()
            .BeEquivalentTo(expectation: packageItem);

        result.Should()
            .BeEquivalentTo(expectation: packageItem);

        packageItemBrokerMock.Verify(
expression: x => x.UpdatePackageItemAsync(updatedPackageItem: It.IsAny<cCoder.Data.Models.Packaging.PackageItem>()),
times: Times.Once
        );

        packageItemBrokerMock.VerifyNoOtherCalls();
    }

}
