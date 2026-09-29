// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System;
using cCoder.Data.Models.CMS;
using cCoder.ContentManagement.Brokers.Storages;



using cCoder.ContentManagement.Services.Foundations.Storages;
using Moq;

namespace cCoder.Core.Services.Tests.CMS.Foundations.Storages;

public partial class AppCultureServiceTests
{
    private readonly Mock<IAppCultureBroker> appCultureBrokerMock;
    private readonly AppCultureService appCultureService;

    public AppCultureServiceTests()
    {
        appCultureBrokerMock = new Mock<IAppCultureBroker>(behavior: MockBehavior.Strict);

        appCultureService = new AppCultureService(
appCultureBroker: appCultureBrokerMock.Object
        );
    }

    private static AppCulture CreateRandomAppCulture(int appId = 1, string cultureId = null)
    {
        AppCulture appCulture = new()
        {
            AppId = appId,
            CultureId = cultureId ?? $"culture-{Guid.NewGuid():N}",
            App = null!,
            Culture = null!,
        };

        return appCulture;
    }
}