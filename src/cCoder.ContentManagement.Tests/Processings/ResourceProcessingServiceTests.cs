// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System;
using cCoder.Data.Models.CMS;
using cCoder.ContentManagement.Services.Foundations.Storages;
using cCoder.ContentManagement.Services.Processings;
using Moq;

namespace cCoder.Core.Services.Tests.CMS.Processings;

public partial class ResourceProcessingServiceTests
{
    private readonly Mock<IResourceService> resourceServiceMock = new();
    private readonly ResourceProcessingService resourceProcessingService;

    public ResourceProcessingServiceTests()
    {
        resourceProcessingService = new ResourceProcessingService(
service: resourceServiceMock.Object
        );
    }

    private static Resource CreateRandomResource(
        int id = 1,
        int appId = 1,
        string culture = "",
        string key = "key"
    ) =>
        new()
        {
            Id = id,
            AppId = appId,
            Name = $"Resource-{Guid.NewGuid():N}",
            Key = key,
            Culture = culture,
            DisplayName = "Display",
            ShortDisplayName = "Display",
            Description = "Description",
        };
}