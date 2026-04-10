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
    public void ShouldDelegateToBrokerWhenGetAll()
    {
        // Given
        PackageItem[] expectedItems = [CreateRandomPackageItem()];
        IQueryable<cCoder.Data.Models.Packaging.PackageItem> packageItems = expectedItems
            .Select(item => item)
            .AsQueryable();

        packageItemBrokerMock.Setup(x => x.GetAllPackageItems(false)).Returns(packageItems);

        // When
        IQueryable<PackageItem> result = packageItemService.GetAll();

        // Then
        result.Should().BeEquivalentTo(expectedItems);
        packageItemBrokerMock.Verify(x => x.GetAllPackageItems(false), Times.Once);
        packageItemBrokerMock.Verify(x => x.GetAppId(It.IsAny<cCoder.Data.Models.Packaging.PackageItem>()), Times.AtMostOnce());
        packageItemBrokerMock.VerifyNoOtherCalls();
        authorizationBrokerMock.VerifyNoOtherCalls();
    }

}



















