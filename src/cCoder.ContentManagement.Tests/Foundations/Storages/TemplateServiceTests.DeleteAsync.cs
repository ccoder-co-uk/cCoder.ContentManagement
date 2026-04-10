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
using CmsDataModels = cCoder.Data.Models.CMS;


namespace cCoder.Core.Services.Tests.CMS.Foundations.Storages;

public partial class TemplateServiceTests
{
    [Fact]
    public async Task ShouldDelegateToBrokerWhenUserIsAuthorizedForDeleteAsync()
    {
        // Given
        Template template = CreateRandomTemplate(id: 5);

        templateBrokerMock.Setup(x => x.GetAllTemplates(false)).Returns(new[] { template }.AsQueryable());

        authorizationBrokerMock.Setup(x => x.Authorize((int?)7, "Template_delete"));
        templateBrokerMock.Setup(x => x.DeleteTemplateAsync(It.IsAny<CmsDataModels.Template>())).ReturnsAsync(1);

        // When
        await templateService.DeleteAsync(5);

        // Then
        templateBrokerMock.Verify(x => x.GetAllTemplates(false), Times.Once);
        templateBrokerMock.Verify(x => x.DeleteTemplateAsync(It.Is<CmsDataModels.Template>(actual => actual.Id == template.Id)), Times.Once);
        templateBrokerMock.VerifyNoOtherCalls();
        authorizationBrokerMock.Verify(x => x.Authorize((int?)7, "Template_delete"), Times.Once);
        authorizationBrokerMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ShouldThrowSecurityExceptionWhenUserLacksDeletePrivilegeForDeleteAsync()
    {
        // Given
        Template template = CreateRandomTemplate(id: 5);

        templateBrokerMock.Setup(x => x.GetAllTemplates(false)).Returns(new[] { template }.AsQueryable());

        authorizationBrokerMock
            .Setup(x => x.Authorize((int?)7, "Template_delete"))
            .Throws(new SecurityException("Access Denied!"));

        // When
        Func<Task> action = async () => await templateService.DeleteAsync(5);

        // Then
        await action.Should().ThrowAsync<SecurityException>().WithMessage("Access Denied!");
        templateBrokerMock.Verify(x => x.GetAllTemplates(false), Times.Once);
        templateBrokerMock.VerifyNoOtherCalls();
        authorizationBrokerMock.Verify(x => x.Authorize((int?)7, "Template_delete"), Times.Once);
        authorizationBrokerMock.VerifyNoOtherCalls();
    }

}















