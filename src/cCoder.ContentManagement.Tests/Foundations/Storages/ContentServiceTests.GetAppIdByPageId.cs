// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using FluentAssertions;
using Moq;
using Xunit;

namespace cCoder.Core.Services.Tests.CMS.Foundations.Storages;

public partial class ContentServiceTests
{
    [Fact]
    public void AppId_WhenPageIdIsValid_IsReturnedFromContentBroker()
    {
        // Given
        int pageId = Random.Shared.Next(minValue: 1, maxValue: int.MaxValue);
        int appId = Random.Shared.Next(minValue: 1, maxValue: int.MaxValue);

        contentBrokerMock
            .Setup(expression: broker => broker.GetAppIdByPageId(pageId))
            .Returns(value: appId);

        // When
        int? result = contentService.GetAppIdByPageId(pageId);

        // Then
        result.Should().Be(appId);
        contentBrokerMock.VerifyAll();
    }
}
