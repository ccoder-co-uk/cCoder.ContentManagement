// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Linq;
using System.Threading.Tasks;
using cCoder.Data.Models.CMS;
using Moq;
using Xunit;
using CmsDataModels = cCoder.Data.Models.CMS;


namespace cCoder.Core.Services.Tests.CMS.Foundations.Storages;

public partial class ScriptServiceTests
{
    [Fact]
    public async Task ShouldDelegateToBrokerWhenDeleteAsync()
    {
        // Given
        Script script = CreateRandomScript(id: 9, appId: 7);

        scriptBrokerMock.Setup(expression: x => x.GetAllScripts())
            .Returns(value: new[] { script }.AsQueryable());

        scriptBrokerMock.Setup(expression: x => x.DeleteScriptAsync(deletedScript: It.IsAny<CmsDataModels.Script>()))
                    .ReturnsAsync(value: 1);

        // When
        await scriptService.DeleteAsync(scriptId: 9);

        // Then
        scriptBrokerMock.Verify(expression: x => x.GetAllScripts(), times: Times.Once);
        scriptBrokerMock.Verify(expression: x => x.DeleteScriptAsync(deletedScript: It.Is<CmsDataModels.Script>(match: actual => actual.Id == script.Id)), times: Times.Once);
        scriptBrokerMock.VerifyNoOtherCalls();
    }
}