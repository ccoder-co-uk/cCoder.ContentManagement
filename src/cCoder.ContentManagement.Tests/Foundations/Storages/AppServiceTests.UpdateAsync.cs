// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System;
using System.Threading.Tasks;
using cCoder.ContentManagement.Models;
using cCoder.Data.Models.CMS;
using System.Security;



using FluentAssertions;
using Moq;
using Xunit;
using CmsDataModels = cCoder.Data.Models.CMS;


namespace cCoder.Core.Services.Tests.CMS.Foundations.Storages;

public partial class AppServiceTests
{
    [Fact]
    public async Task ShouldDelegateSingleAppRowToBrokerForUpdateAppOperationAsync()
    {
        // Given
        App app = CreateRandomApp(id: 5);

        CmsDataModels.App submitted = null;

        appBrokerMock
            .Setup(expression: x => x.UpdateAppAsync(updatedApp: It.IsAny<CmsDataModels.App>()))
            .Callback<CmsDataModels.App>(action: candidate => submitted = candidate)
            .ReturnsAsync(valueFunction: (CmsDataModels.App value) => value);

        // When
        App result = (await appService.UpdateAppOperationAsync(updatedAppOperation: new AppOperation { App = app })).App;

        // Then

        result.Should()
            .BeSameAs(expected: app);

        submitted.Should()
            .NotBeNull();

        submitted.Should()
            .NotBeSameAs(unexpected: app);

        result.Should()
            .NotBeSameAs(unexpected: submitted);

        result.Should()
            .BeEquivalentTo(expectation: new
            {
                app.Id,
                app.DefaultCultureId,
                app.TenantId,
                app.Name,
                app.Domain,
                app.DefaultTheme,
                app.ConfigJson
            });

        submitted.Should()
            .BeEquivalentTo(expectation: new
            {
                app.Id,
                app.DefaultCultureId,
                app.TenantId,
                app.Name,
                app.Domain,
                app.DefaultTheme,
                app.ConfigJson
            });

        submitted.Roles.Should()
            .BeNull();

        result.Roles.Should()
            .BeNull();

        appBrokerMock.Verify(expression: x => x.UpdateAppAsync(updatedApp: It.IsAny<CmsDataModels.App>()), times: Times.Once);
        appBrokerMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ShouldTranslateSecurityExceptionFromBrokerForUpdateAppOperationAsync()
    {
        // Given
        App app = CreateRandomApp(id: 5);

        appBrokerMock
            .Setup(expression: x => x.UpdateAppAsync(updatedApp: It.IsAny<App>()))
            .Throws(exception: new SecurityException(message: "Access Denied!"));

        // When
        Func<Task> action = async () => await appService.UpdateAppOperationAsync(updatedAppOperation: new AppOperation { App = app });

        // Then

        await action.Should()
            .ThrowAsync<SecurityException>()
            .WithMessage(expectedWildcardPattern: "Access Denied!");

        appBrokerMock.Verify(
            expression: x => x.UpdateAppAsync(updatedApp: It.IsAny<App>()),
            times: Times.Once);

        appBrokerMock.VerifyNoOtherCalls();
    }

}