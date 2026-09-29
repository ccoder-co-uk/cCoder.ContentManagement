// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Threading.Tasks;
using cCoder.Data.Models.CMS;
using Moq;
using Xunit;
using CmsDataModels = cCoder.Data.Models.CMS;

#pragma warning disable STXFORMAT005, STXFORMAT008


namespace cCoder.Core.Services.Tests.CMS.Foundations.Storages;

public partial class CultureServiceTests
{
    [Fact]
    public async Task ShouldDelegateToBrokerWhenUserIsAuthorizedForAddAsync()
    {
        // Given
        Culture culture = CreateRandomCulture();

        CmsDataModels.Culture submitted = null;

        cultureBrokerMock.Setup(expression: x => x.AddCultureAsync(newCulture: It.IsAny<CmsDataModels.Culture>()))
            .Callback<CmsDataModels.Culture>(action: candidate => submitted = candidate)
            .ReturnsAsync(valueFunction: (CmsDataModels.Culture value) => value);

        // When
        Culture result = await cultureService.AddCultureAsync(newCulture: culture);

        // Then

        Assert.Same(expected: culture, actual: result);
        Assert.NotNull(@object: submitted);
        Assert.NotSame(expected: culture, actual: submitted);
        Assert.NotSame(expected: submitted, actual: result);
        Assert.Equal(expected: culture.Name, actual: submitted.Name);
        Assert.Equal(expected: culture.Id, actual: submitted.Id);
        Assert.Null(@object: submitted.Apps);
        Assert.Null(@object: submitted.MetaItems);
        Assert.Null(@object: submitted.PageContents);
        Assert.Null(@object: submitted.PageInfos);
        Assert.Null(@object: submitted.Users);
        Assert.Equal(expected: culture.Name, actual: result.Name);
        Assert.Equal(expected: culture.Id, actual: result.Id);
        Assert.Same(expected: culture.Apps, actual: result.Apps);
        Assert.Same(expected: culture.MetaItems, actual: result.MetaItems);
        Assert.Same(expected: culture.PageContents, actual: result.PageContents);
        Assert.Same(expected: culture.PageInfos, actual: result.PageInfos);
        Assert.Same(expected: culture.Users, actual: result.Users);

        cultureBrokerMock.Verify(expression: x => x.AddCultureAsync(
            newCulture: It.IsAny<CmsDataModels.Culture>()), times: Times.Once);

        cultureBrokerMock.VerifyNoOtherCalls();
    }

}