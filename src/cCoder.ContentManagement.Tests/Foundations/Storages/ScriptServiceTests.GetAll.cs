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
using CmsDataModels = cCoder.Data.Models.CMS;


namespace cCoder.Core.Services.Tests.CMS.Foundations.Storages;

public partial class ScriptServiceTests
{
    [Fact]
    public void ShouldDelegateToBrokerWhenGetAll()
    {
        // Given
        Script[] expectedItems = [CreateRandomScript()];
        IQueryable<CmsDataModels.Script> scripts = expectedItems
            .Select(item => item)
            .AsQueryable();

        scriptBrokerMock.Setup(x => x.GetAllScripts(false)).Returns(scripts);

        // When
        IQueryable<Script> result = scriptService.GetAll();

        // Then
        result.Should().BeEquivalentTo(expectedItems);
        scriptBrokerMock.Verify(x => x.GetAllScripts(false), Times.Once);
        scriptBrokerMock.VerifyNoOtherCalls();
        authorizationBrokerMock.VerifyNoOtherCalls();
    }

}

















