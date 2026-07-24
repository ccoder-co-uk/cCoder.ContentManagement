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

public partial class PackageProcessingServiceTests
{
    [Fact]
    public void ShouldDelegateToFoundationServiceWhenGet()
    {
        // Given
        Package entity = CreateRandomPackage();
        var id = entity.Id;

        packageServiceMock.Setup(expression: x => x.GetPackage(packageId: id))
            .Returns(value: entity);

        // When
        Package result = packageProcessingService.GetPackage(packageId: id);

        // Then

        result.Should()
            .BeSameAs(expected: entity);

        packageServiceMock.Verify(expression: x => x.GetPackage(packageId: id), times: Times.Once);
        packageServiceMock.VerifyNoOtherCalls();
    }

}