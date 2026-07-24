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



namespace cCoder.Core.Services.Tests.CMS.Processings;

public partial class PackageItemProcessingServiceTests
{
    [Fact]
    public async Task ShouldDelegateToFoundationServiceWhenUpdateAsync()
    {
        // Given
        PackageItem entity = CreateRandomPackageItem();

        packageItemServiceMock.Setup(expression: x => x.UpdatePackageItemAsync(updatedPackageItem: entity))
            .ReturnsAsync(value: entity);

        // When
        PackageItem result = await packageItemProcessingService.UpdatePackageItemAsync(updatedPackageItem: entity);

        // Then

        result.Should()
            .BeSameAs(expected: entity);

        packageItemServiceMock.Verify(expression: x => x.UpdatePackageItemAsync(updatedPackageItem: entity), times: Times.Once);
        packageItemServiceMock.VerifyNoOtherCalls();
    }

}