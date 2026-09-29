// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.CMS;
using FluentAssertions;
using Moq;
using Xunit;


namespace cCoder.Core.Services.Tests.CMS.Processings;

public partial class AppProcessingServiceTests
{
    [Fact]
    public void ShouldDelegateToFoundationServiceWhenGet()
    {
        // Given
        App entity = CreateRandomApp();
        var id = entity.Id;

        appServiceMock.Setup(expression: x => x.GetApp(appId: id))
            .Returns(value: entity);

        // When
        App result = appProcessingService.GetApp(appId: id);

        // Then

        result.Should()
            .BeSameAs(expected: entity);

        appServiceMock.Verify(expression: x => x.GetApp(appId: id), times: Times.Once);
        appServiceMock.VerifyNoOtherCalls();
    }

}