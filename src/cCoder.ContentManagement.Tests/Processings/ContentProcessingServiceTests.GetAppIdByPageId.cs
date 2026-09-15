// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using FluentAssertions;
using Moq;
using Xunit;

namespace cCoder.Core.Services.Tests.CMS.Processings;

public partial class ContentProcessingServiceTests
{
    [Fact]
    public void AppId_WhenPageIdIsValid_IsReturnedFromContentService()
    {
        // Given
        int pageId = Random.Shared.Next(minValue: 1, maxValue: int.MaxValue);
        int appId = Random.Shared.Next(minValue: 1, maxValue: int.MaxValue);

        contentServiceMock
            .Setup(expression: service => service.GetAppIdByPageId(pageId: pageId))
            .Returns(value: appId);

        // When
        int? result = contentProcessingService.GetAppIdByPageId(pageId: pageId);

        // Then
        result.Should()
            .Be(expected: appId);

        contentServiceMock.VerifyAll();
    }
}