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
    public void ShouldDelegateToBrokerWhenGetAll()
    {
        // Given
        Package[] expectedItems = [CreateRandomPackage()];

        IQueryable<cCoder.Data.Models.Packaging.Package> packages = expectedItems
            .Select(selector: item => item)
            .AsQueryable();

        packageBrokerMock.Setup(expression: x => x.GetAllPackages(ignoreFilters: false))
            .Returns(value: packages);

        // When
        IQueryable<Package> result = packageService.GetAllPackage();

        // Then

        result.Should()
            .BeEquivalentTo(expectation: expectedItems);

        packageBrokerMock.Verify(expression: x => x.GetAllPackages(ignoreFilters: false), times: Times.Once);
        packageBrokerMock.VerifyNoOtherCalls();
        authorizationBrokerMock.VerifyNoOtherCalls();
    }

}