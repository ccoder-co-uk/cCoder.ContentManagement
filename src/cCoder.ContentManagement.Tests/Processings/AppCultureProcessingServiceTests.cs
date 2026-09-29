// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System;
using cCoder.Data.Models.CMS;
using cCoder.ContentManagement.Services.Foundations.Storages;
using cCoder.ContentManagement.Services.Processings;
using Moq;


namespace cCoder.Core.Services.Tests.CMS.Processings;

public partial class AppCultureProcessingServiceTests
{
    private readonly Mock<IAppCultureService> appCultureServiceMock = new();
    private readonly AppCultureProcessingService appCultureProcessingService;

    public AppCultureProcessingServiceTests()
    {
        appCultureProcessingService = new AppCultureProcessingService(service: appCultureServiceMock.Object);
    }

    private static AppCulture CreateRandomAppCulture() =>
        new()
        {
            AppId = 1,
            CultureId = $"culture-{Guid.NewGuid():N}",
            App = null!,
            Culture = null!,
        };
}