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



namespace cCoder.Core.Services.Tests.CMS.Processings;

public partial class PackageItemProcessingServiceTests
{
    [Fact]
    public void ShouldDelegateToFoundationServiceWhenGetAll()
    {
        // Given
        IQueryable<PackageItem> entities = new[] { CreateRandomPackageItem() }.AsQueryable();

        packageItemServiceMock.Setup(expression: x => x.GetAllPackageItem())
            .Returns(value: entities);

        // When
        IQueryable<PackageItem> result = packageItemProcessingService.GetAllPackageItem();

        // Then

        result.Should()
            .BeSameAs(expected: entities);

        packageItemServiceMock.Verify(expression: x => x.GetAllPackageItem(), times: Times.Once);
        packageItemServiceMock.VerifyNoOtherCalls();
    }

}