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
        // Given
        CommonObject commonObject = CreateRandomCommonObject(type: "Core/Other");
        CommonObject existingVersion = CreateRandomCommonObject(type: commonObject.Type);
        existingVersion.Name = commonObject.Name;
        existingVersion.Culture = commonObject.Culture;
        existingVersion.Key = commonObject.Key;
        existingVersion.Version = 2;

        commonObjectServiceMock
            .Setup(expression: service => service.GetAllCommonObject(ignoreFilters: false))
            .Returns(value: new[] { existingVersion }.AsQueryable());

        commonObjectServiceMock
            .Setup(expression: service => service.AddCommonObjectAsync(
                newCommonObject: commonObject,
                userId: CurrentUserId))
            .ReturnsAsync(value: commonObject);

        // When
        CommonObject result = await commonObjectProcessingService.UpdateCommonObjectAsync(
            updatedCommonObject: commonObject,
            userId: CurrentUserId);

        // Then
        result.Id.Should()
            .Be(expected: 0);

        result.Version.Should()
            .Be(expected: 3);

        result.CreatedBy.Should()
            .Be(expected: CurrentUserId);

        result.LastUpdatedBy.Should()
            .Be(expected: CurrentUserId);

        commonObjectServiceMock.Verify(
            expression: service => service.GetAllCommonObject(ignoreFilters: false),
            times: Times.Exactly(callCount: 2));

        commonObjectServiceMock.VerifyAll();
        VerifyNoOtherCommonObjectServiceCalls();
    }
}