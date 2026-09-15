// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models;
using FluentAssertions;
using Moq;
using Xunit;

namespace cCoder.Core.Services.Tests.CMS.Processings;

public partial class CommonObjectProcessingServiceTests
{
    [Fact]
    public async Task ShouldCreateNextVersionAndAddItWhenUpdateAsync()
    {
        CommonObject commonObject = CreateRandomCommonObject(type: "Core/Other");
        CommonObject existingVersion = CreateRandomCommonObject(type: commonObject.Type);
        existingVersion.Name = commonObject.Name;
        existingVersion.Culture = commonObject.Culture;
        existingVersion.Key = commonObject.Key;
        existingVersion.Version = 2;

        commonObjectServiceMock.Setup(service => service.GetAllCommonObject(false))
            .Returns(new[] { existingVersion }.AsQueryable());
        commonObjectServiceMock
            .Setup(service => service.AddCommonObjectAsync(commonObject, CurrentUserId))
            .ReturnsAsync(commonObject);

        CommonObject result = await commonObjectProcessingService.UpdateCommonObjectAsync(
            updatedCommonObject: commonObject,
            userId: CurrentUserId);

        result.Id.Should().Be(0);
        result.Version.Should().Be(3);
        result.CreatedBy.Should().Be(CurrentUserId);
        result.LastUpdatedBy.Should().Be(CurrentUserId);
        commonObjectServiceMock.Verify(service => service.GetAllCommonObject(false), Times.Exactly(2));
        commonObjectServiceMock.VerifyAll();
        VerifyNoOtherCommonObjectServiceCalls();
    }
}