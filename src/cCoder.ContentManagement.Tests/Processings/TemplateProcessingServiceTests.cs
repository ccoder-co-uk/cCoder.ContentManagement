// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System;
using cCoder.Data.Models.CMS;
using cCoder.ContentManagement.Services.Foundations.Storages;
using cCoder.ContentManagement.Services.Processings;
using Moq;


namespace cCoder.Core.Services.Tests.CMS.Processings;

public partial class TemplateProcessingServiceTests
{
    private readonly Mock<ITemplateService> templateServiceMock = new();
    private readonly TemplateProcessingService templateProcessingService;

    public TemplateProcessingServiceTests()
    {
        templateProcessingService = new TemplateProcessingService(
            service: templateServiceMock.Object);
    }

    private static Template CreateRandomTemplate() =>
        new()
        {
            Id = Random.Shared.Next(minValue: 1, maxValue: 10000),
            AppId = 1,
            Name = $"Template-{Guid.NewGuid():N}",
            ResourceKey = "template",
            RawString = "<html></html>",
        };
}