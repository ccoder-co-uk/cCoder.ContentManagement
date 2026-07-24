// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Services.Foundations.Storages;
using cCoder.ContentManagement.Services.Processings;
using cCoder.Data.Models.CMS;
using FizzWare.NBuilder;
using Moq;

namespace cCoder.Core.Services.Tests.CMS.Processings;

public partial class AppUserProcessingServiceTests
{
    private readonly Mock<IAppService> appServiceMock = new();
    private readonly AppUserProcessingService appUserProcessingService;

    public AppUserProcessingServiceTests()
    {
        appUserProcessingService = new AppUserProcessingService(
            appService: appServiceMock.Object);
    }

    private static App CreateRandomApp() =>
        Builder<App>
            .CreateNew()
            .With(
                func: app =>
                    app.Id = Random.Shared.Next(
                        minValue: 1,
                        maxValue: 10000))
            .With(func: app => app.Roles = [])
            .Build();
}