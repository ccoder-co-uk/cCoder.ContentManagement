using cCoder.Data.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Packaging;
using cCoder.Data.Models.Security;
using ComponentRenderParams = cCoder.ContentManagement.Models.ComponentRenderParams;
using Config = cCoder.ContentManagement.Models.Config;
using PageRenderParams = cCoder.ContentManagement.Models.PageRenderParams;
using PageRoleInfo = cCoder.ContentManagement.Models.PageRoleInfo;
using RenderParams = cCoder.ContentManagement.Models.RenderParams;
using RenderResult = cCoder.ContentManagement.Models.RenderResult;
using TemplateRenderParams = cCoder.ContentManagement.Models.TemplateRenderParams;
using System.Security;



using FluentAssertions;
using Moq;
using Xunit;
using SecurityDataModels = cCoder.Data.Models.Security;


namespace cCoder.Core.Services.Tests.CMS.Processings;

public partial class AppProcessingServiceTests
{
    [Fact]
    public async Task ShouldDefaultThemeAndReturnAddedAppWhenUserCanCreateForAddAsync()
    {
        // Given
        authorizationBrokerMock
            .Setup(x => x.Authorize(It.IsAny<int?>(), It.IsAny<string>()))
            .Callback((int? appId, string privilege) =>
            {
                if (!(currentUser?.Can(appId, privilege) ?? false))
                    throw new SecurityException("Access Denied!");
            });

        authorizationBrokerMock
            .Setup(x => x.IsAdminOfApp(It.IsAny<int>()))
            .Returns((int appId) => currentUser?.IsAdminOfApp(appId) ?? false);

        authorizationBrokerMock.Setup(x => x.GetCurrentUser()).Returns(() => currentUser);

        User actor = TestUsers.WithPrivilege("app_create");
        App inputApp = CreateRandomApp();
        inputApp.DefaultTheme = string.Empty;
        inputApp.Cultures = [];
        inputApp.Roles =
        [
            new Role
            {
                Id = Guid.NewGuid(),
                AppId = inputApp.Id,
                Name = "Administrators",
                Privs = "app_admin,app_delete"
            }
        ];
        currentUser = actor;
        appServiceMock.Setup(x => x.AddAsync(It.IsAny<App>()))
            .ReturnsAsync((App candidate) =>
            {
                candidate.Id = 1;
                return candidate;
            });
        appEventProcessingServiceMock
            .Setup(x => x.RaiseAppAddEventAsync(It.IsAny<App>()))
            .Returns(ValueTask.CompletedTask);

        cultureServiceMock
            .Setup(x => x.GetAll(false))
            .Returns(new[] { new Culture { Id = string.Empty } }.AsQueryable());
        privilegeBrokerMock
            .Setup(x => x.GetAllPrivileges(false))
            .Returns(
                new[]
                {
                    new SecurityDataModels.Privilege { Id = "app_create", Operation = "Create", Type = "App" },
                    new SecurityDataModels.Privilege { Id = "app_read", Operation = "Read", Type = "App" },
                    new SecurityDataModels.Privilege { Id = "app_update", Operation = "Update", Type = "App" },
                    new SecurityDataModels.Privilege { Id = "app_delete", Operation = "Delete", Type = "App" }
                }.AsQueryable());

        // When
        App result = await appProcessingService.AddAsync(inputApp);

        // Then
        result.Id.Should().Be(1);
        result.DefaultTheme.Should().Be("Default");
        result.Cultures.Should().HaveCount(1);
        result.Roles.Should().HaveCount(3);
        appServiceMock.Verify(
                x =>
                    x.AddAsync(
                        It.Is<App>(app =>
                            app.DefaultTheme == "Default"
                            && app.Cultures.Count == 1
                            && app.Roles.Count == 3
                            && app.Roles.Any(role => role.Name == "Administrators")
                            && app.Roles.Any(role => role.Name == "Users")
                            && app.Roles.Any(role => role.Name == "Guests")
                        )
                    ),
            Times.Once
        );
        cultureServiceMock.Verify(x => x.GetAll(false), Times.Once);
        privilegeBrokerMock.Verify(x => x.GetAllPrivileges(false), Times.Exactly(2));
        appServiceMock.VerifyNoOtherCalls();
        cultureServiceMock.VerifyNoOtherCalls();
        privilegeBrokerMock.VerifyNoOtherCalls();
        appEventProcessingServiceMock.Verify(x => x.RaiseAppAddEventAsync(It.IsAny<App>()), Times.Once);
        appEventProcessingServiceMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ShouldThrowSecurityExceptionWhenAddAsync()
    {
        // Given
        authorizationBrokerMock
            .Setup(x => x.Authorize(It.IsAny<int?>(), It.IsAny<string>()))
            .Callback((int? appId, string privilege) =>
            {
                if (!(currentUser?.Can(appId, privilege) ?? false))
                    throw new SecurityException("Access Denied!");
            });

        authorizationBrokerMock
            .Setup(x => x.IsAdminOfApp(It.IsAny<int>()))
            .Returns((int appId) => currentUser?.IsAdminOfApp(appId) ?? false);

        authorizationBrokerMock.Setup(x => x.GetCurrentUser()).Returns(() => currentUser);

        App app = new() { Name = "App", Domain = "app.local" };

        currentUser = TestUsers.WithoutPrivileges();
        cultureServiceMock
            .Setup(x => x.GetAll(false))
            .Returns(new[] { new Culture { Id = string.Empty } }.AsQueryable());
        privilegeBrokerMock
            .Setup(x => x.GetAllPrivileges(false))
            .Returns(Array.Empty<SecurityDataModels.Privilege>().AsQueryable());
        appServiceMock
            .Setup(x => x.AddAsync(It.IsAny<App>()))
            .ThrowsAsync(new SecurityException("Access Denied!"));

        // When
        Func<Task> act = async () => await appProcessingService.AddAsync(app);

        // Then
        await act.Should().ThrowAsync<SecurityException>().WithMessage("Access Denied!");
        cultureServiceMock.Verify(x => x.GetAll(false), Times.Once);
        privilegeBrokerMock.Verify(x => x.GetAllPrivileges(false), Times.Exactly(2));
        appServiceMock.Verify(x => x.AddAsync(It.IsAny<App>()), Times.Once);
        appServiceMock.VerifyNoOtherCalls();
        appEventProcessingServiceMock.VerifyNoOtherCalls();
    }

}
















