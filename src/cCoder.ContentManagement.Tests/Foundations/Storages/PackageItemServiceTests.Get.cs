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
using FluentAssertions;
using Moq;
using Xunit;



namespace cCoder.Core.Services.Tests.CMS.Foundations.Storages;

public partial class PackageItemServiceTests
{
    [Fact]
    public void ShouldDelegateToBrokerWhenGet()
    {
        // Given
        Guid packageItemId = Guid.NewGuid();
        PackageItem packageItem = CreateRandomPackageItem(id: packageItemId);

        packageItemBrokerMock.Setup(expression: x => x.GetAllPackageItems(ignoreFilters: false))
            .Returns(value: new[] { packageItem }.AsQueryable());

        // When
        PackageItem result = packageItemService.GetPackageItem(packageItemId: packageItemId);

        // Then

        result.Should()
            .BeEquivalentTo(expectation: packageItem);

        packageItemBrokerMock.Verify(expression: x => x.GetAllPackageItems(ignoreFilters: false), times: Times.Once);
        packageItemBrokerMock.Verify(expression: x => x.GetAppId(entity: It.IsAny<cCoder.Data.Models.Packaging.PackageItem>()), times: Times.AtMostOnce());
        packageItemBrokerMock.VerifyNoOtherCalls();
        authorizationBrokerMock.VerifyNoOtherCalls();
    }

}