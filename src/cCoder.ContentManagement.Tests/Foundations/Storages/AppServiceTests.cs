// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System;
using cCoder.Data.Models.CMS;
using cCoder.ContentManagement.Brokers.Storages;



using cCoder.ContentManagement.Services.Foundations.Storages;
using FizzWare.NBuilder;
using Moq;

namespace cCoder.Core.Services.Tests.CMS.Foundations.Storages;

public partial class AppServiceTests
{
    private readonly Mock<IAppBroker> appBrokerMock;
    private readonly AppService appService;

    public AppServiceTests()
    {
        appBrokerMock = new Mock<IAppBroker>(behavior: MockBehavior.Strict);
        appService = new AppService(
            appBroker: appBrokerMock.Object);
    }

    private static App CreateRandomApp(int id = 42)
    {
        App app = Builder<App>
            .CreateNew()
            .With(func: x => x.Id = id)
            .With(func: x => x.DefaultCultureId = "en-GB")
            .With(func: x => x.TenantId = $"tenant-{Guid.NewGuid():N}")
            .With(func: x => x.Name = $"App-{Guid.NewGuid():N}")
            .With(func: x => x.Domain = $"app-{Guid.NewGuid():N}.test")
            .With(func: x => x.DefaultTheme = "default")
            .With(func: x => x.ConfigJson = "{}")
            .Build();

        return app;
    }
}