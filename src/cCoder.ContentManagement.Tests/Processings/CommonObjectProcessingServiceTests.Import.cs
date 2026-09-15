// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Models;
using cCoder.Data.Models;
using FluentAssertions;
using Moq;
using Xunit;

namespace cCoder.Core.Services.Tests.CMS.Processings;

public partial class CommonObjectProcessingServiceTests
{
    [Fact]
    public async Task ShouldResetIdentityAndAddWhenImportItemIsNew()
    {
        CommonObject incoming = CreateRandomCommonObject(type: "Core/Other");
        incoming.Id = 99;
        incoming.Version = 9;

        commonObjectServiceMock
            .Setup(service => service.AddCommonObjectAsync(incoming, CurrentUserId))
            .ReturnsAsync(incoming);

        OperationResult<CommonObject>[] results = (await commonObjectProcessingService
            .AddAllCommonObjectsAsync([incoming], [], CurrentUserId)).ToArray();

        results.Should().ContainSingle(result => result.Success && result.Item == incoming);
        incoming.Id.Should().Be(0);
        incoming.Version.Should().Be(1);
        commonObjectServiceMock.VerifyAll();
        VerifyNoOtherCommonObjectServiceCalls();
    }

    [Fact]
    public async Task ShouldPromoteVersionAndAddWhenImportItemIsNewer()
    {
        CommonObject existing = CreateRandomCommonObject(type: "Core/Other");
        existing.Version = 4;
        existing.CreatedOn = DateTimeOffset.UtcNow.AddHours(-2);
        existing.LastUpdated = existing.CreatedOn;
        CommonObject incoming = CreateRandomCommonObject(type: existing.Type);
        incoming.Name = existing.Name;
        incoming.Key = existing.Key;
        incoming.Culture = existing.Culture;
        incoming.CreatedOn = DateTimeOffset.UtcNow;
        incoming.LastUpdated = incoming.CreatedOn;

        commonObjectServiceMock.Setup(service => service.GetAllCommonObject(false))
            .Returns(new[] { existing }.AsQueryable());
        commonObjectServiceMock
            .Setup(service => service.AddCommonObjectAsync(incoming, CurrentUserId))
            .ReturnsAsync(incoming);

        OperationResult<CommonObject>[] results = (await commonObjectProcessingService
            .AddAllCommonObjectsAsync([incoming], [existing], CurrentUserId)).ToArray();

        results.Should().ContainSingle(result => result.Success);
        incoming.Id.Should().Be(0);
        incoming.Version.Should().Be(5);
        commonObjectServiceMock.Verify(service => service.GetAllCommonObject(false), Times.Exactly(2));
        commonObjectServiceMock.VerifyAll();
        VerifyNoOtherCommonObjectServiceCalls();
    }

    [Fact]
    public async Task ShouldReturnNoResultsWhenImportItemIsNotNewer()
    {
        CommonObject existing = CreateRandomCommonObject(type: "Core/Other");
        CommonObject incoming = CreateRandomCommonObject(type: existing.Type);
        incoming.Name = existing.Name;
        incoming.Key = existing.Key;
        incoming.Culture = existing.Culture;
        incoming.CreatedOn = existing.CreatedOn;
        incoming.LastUpdated = existing.LastUpdated;

        OperationResult<CommonObject>[] results = (await commonObjectProcessingService
            .AddAllCommonObjectsAsync([incoming], [existing], CurrentUserId)).ToArray();

        results.Should().BeEmpty();
        VerifyNoOtherCommonObjectServiceCalls();
    }
}