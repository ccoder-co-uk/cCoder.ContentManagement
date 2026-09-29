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

public partial class LayoutServiceTests
{
    [Fact]
    public async Task ShouldDelegateToBrokerWhenDeleteAsync()
    {
        // Given
        Layout layout = CreateRandomLayout(id: 9, appId: 7);

        layoutBrokerMock.Setup(expression: x => x.GetAllLayouts())
            .Returns(value: new[] { layout }.AsQueryable());

        layoutBrokerMock.Setup(expression: x => x.DeleteLayoutAsync(deletedLayout: It.IsAny<CmsDataModels.Layout>()))
                    .ReturnsAsync(value: 1);

        // When
        await layoutService.DeleteAsync(layoutId: 9);

        // Then
        layoutBrokerMock.Verify(expression: x => x.GetAllLayouts(), times: Times.Once);
        layoutBrokerMock.Verify(expression: x => x.DeleteLayoutAsync(deletedLayout: It.Is<CmsDataModels.Layout>(match: actual => actual.Id == layout.Id)), times: Times.Once);
        layoutBrokerMock.VerifyNoOtherCalls();
    }
}