// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System;
using cCoder.Data.Models.CMS;
using cCoder.ContentManagement.Services.Foundations.Storages;
using cCoder.ContentManagement.Services.Processings;
using Moq;


namespace cCoder.Core.Services.Tests.CMS.Processings;

public partial class PageInfoProcessingServiceTests
{
    private readonly Mock<IPageInfoService> pageInfoServiceMock = new();
    private readonly PageInfoProcessingService pageInfoProcessingService;

    public PageInfoProcessingServiceTests()
    {
        pageInfoProcessingService = new PageInfoProcessingService(
            service: pageInfoServiceMock.Object);
    }

    private static PageInfo CreateRandomPageInfo() =>
        new()
        {
            Id = Random.Shared.Next(minValue: 1, maxValue: 10000),
            PageId = Random.Shared.Next(minValue: 1, maxValue: 10000),
            CultureId = "en-GB",
            Title = $"Title-{Guid.NewGuid():N}",
            Description = "Description",
            Keywords = "Keywords",
            Page = null!,
            Culture = null!,
        };
}