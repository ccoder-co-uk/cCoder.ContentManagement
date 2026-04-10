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
    public void ShouldDelegateToFoundationServiceWhenGet()
    {
        // Given
        PackageItem entity = CreateRandomPackageItem();
        var id = entity.Id;
        packageItemServiceMock.Setup(x => x.Get(id)).Returns(entity);

        // When
        PackageItem result = packageItemProcessingService.Get(id);

        // Then
        result.Should().BeSameAs(entity);
        packageItemServiceMock.Verify(x => x.Get(id), Times.Once);
        packageItemServiceMock.VerifyNoOtherCalls();
    }

}














