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

public partial class PackageServiceTests
{
    [Fact]
    public void ShouldDelegateToBrokerWhenGet()
    {
        // Given
        Guid packageId = Guid.NewGuid();
        Package package = CreateRandomPackage(id: packageId);

        packageBrokerMock.Setup(expression: x => x.GetAllPackages(ignoreFilters: false))
            .Returns(value: new[] { package }.AsQueryable());

        // When
        Package result = packageService.GetPackage(packageId: packageId);

        // Then

        result.Should()
            .BeEquivalentTo(expectation: package);

        packageBrokerMock.Verify(expression: x => x.GetAllPackages(ignoreFilters: false), times: Times.Once);
        packageBrokerMock.VerifyNoOtherCalls();
        authorizationBrokerMock.VerifyNoOtherCalls();
    }

}