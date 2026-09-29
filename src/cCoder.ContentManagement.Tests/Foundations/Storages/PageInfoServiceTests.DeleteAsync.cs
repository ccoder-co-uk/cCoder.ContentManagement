// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Linq;
using System.Threading.Tasks;
using cCoder.Data.Models.CMS;
using Moq;
using Xunit;
using DataPageInfo = cCoder.Data.Models.CMS.PageInfo;


namespace cCoder.Core.Services.Tests.CMS.Foundations.Storages;

public partial class PageInfoServiceTests
{
    [Fact]
    public async Task ShouldDelegateToBrokerWhenUserIsAuthorizedForDeleteAsync()
    {
        // Given
        PageInfo pageInfo = CreateRandomPageInfo(id: 9);

        pageInfoBrokerMock.Setup(expression: x => x.GetAllPageInfo())
            .Returns(value: new[] { ToDataPageInfo(pageInfo: pageInfo) }.AsQueryable());

        pageInfoBrokerMock.Setup(expression: x => x.DeletePageInfoAsync(deletedPageInfo: It.Is<DataPageInfo>(match: candidate => candidate.Id == pageInfo.Id)))
            .ReturnsAsync(value: 1);

        // When
        await pageInfoService.DeleteAsync(pageInfoId: 9);

        // Then
        pageInfoBrokerMock.Verify(expression: x => x.GetAllPageInfo(), times: Times.Once);
        pageInfoBrokerMock.Verify(expression: x => x.DeletePageInfoAsync(deletedPageInfo: It.Is<DataPageInfo>(match: candidate => candidate.Id == pageInfo.Id)), times: Times.Once);
        pageInfoBrokerMock.VerifyNoOtherCalls();
    }

}