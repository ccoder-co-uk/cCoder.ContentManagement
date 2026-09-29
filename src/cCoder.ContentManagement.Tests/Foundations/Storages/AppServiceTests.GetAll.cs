// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Linq;
using cCoder.ContentManagement.Models;
using cCoder.Data.Models.CMS;
using FluentAssertions;
using Moq;
using Xunit;
using CmsDataModels = cCoder.Data.Models.CMS;


namespace cCoder.Core.Services.Tests.CMS.Foundations.Storages;

public partial class AppServiceTests
{
    [Fact]
    public void ShouldReturnAppsWhenGetAll()
    {
        // Given
        App[] expectedApps = [CreateRandomApp(id: 1)];

        IQueryable<CmsDataModels.App> apps = expectedApps.Select(selector: app => app)
            .AsQueryable();

        appBrokerMock.Setup(expression: x => x.GetAllApps())
            .Returns(value: apps);

        // When
        IQueryable<App> result = appService.GetVisibleAppsAppOperation(appOperation: new AppOperation()).Apps.AsQueryable();

        // Then

        result.Should()
            .BeEquivalentTo(expectation: expectedApps);

        appBrokerMock.Verify(expression: x => x.GetAllApps(), times: Times.Once);
        appBrokerMock.VerifyNoOtherCalls();
    }

}