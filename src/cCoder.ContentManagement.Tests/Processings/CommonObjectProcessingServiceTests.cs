// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models;
using cCoder.ContentManagement.Services.Foundations.Storages;
using cCoder.ContentManagement.Services.Processings;
using FizzWare.NBuilder;
using Moq;

namespace cCoder.Core.Services.Tests.CMS.Processings;

public partial class CommonObjectProcessingServiceTests
{
    private readonly Mock<ICommonObjectService> commonObjectServiceMock = new();
    private readonly CommonObjectProcessingService commonObjectProcessingService;
    private const string CurrentUserId = "test-user";

    public CommonObjectProcessingServiceTests()
    {
        commonObjectProcessingService = new CommonObjectProcessingService(
            service: commonObjectServiceMock.Object);
    }

    private static CommonObject CreateRandomCommonObject(
        string type = "ContentManagement/Resource"
    ) =>
        Builder<CommonObject>
            .CreateNew()
        .With(func: x => x.Id = Random.Shared.Next(minValue: 1, maxValue: 10000))
        .With(func: x => x.Name = $"CommonObject-{Guid.NewGuid():N}")
        .With(func: x => x.Key = $"key-{Guid.NewGuid():N}")
        .With(func: x => x.Culture = "en-GB")
        .With(func: x => x.Type = type)
        .With(func: x => x.Json = "{}")
        .With(func: x => x.Version = 1)
        .With(func: x => x.CreatedBy = "seed-user")
        .With(func: x => x.LastUpdatedBy = "seed-user")
        .With(func: x => x.CreatedOn = DateTimeOffset.UtcNow.AddMinutes(minutes: -5))
        .With(func: x => x.LastUpdated = DateTimeOffset.UtcNow.AddMinutes(minutes: -5))
        .Build();

    private void VerifyNoOtherCommonObjectServiceCalls()
    {
        commonObjectServiceMock.VerifyNoOtherCalls();
    }
}