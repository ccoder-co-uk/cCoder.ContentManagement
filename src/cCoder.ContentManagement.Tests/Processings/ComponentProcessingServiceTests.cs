// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System;
using cCoder.Data.Models.CMS;
using cCoder.ContentManagement.Services.Foundations.Storages;
using cCoder.ContentManagement.Services.Processings;
using Moq;


namespace cCoder.Core.Services.Tests.CMS.Processings;

public partial class ComponentProcessingServiceTests
{
    private readonly Mock<IComponentService> componentServiceMock = new();
    private readonly ComponentProcessingService componentProcessingService;

    public ComponentProcessingServiceTests()
    {
        componentProcessingService = new ComponentProcessingService(service: componentServiceMock.Object);
    }

    private static Component CreateRandomComponent() =>
        new()
        {
            Id = Random.Shared.Next(minValue: 1, maxValue: 10000),
            AppId = 1,
            Name = $"Component-{Guid.NewGuid():N}",
            ResourceKey = "component",
            Content = "<div>content</div>",
            Script = "console.log('component');",
            Key = $"key-{Guid.NewGuid():N}",
        };

}