// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.ComponentModel.DataAnnotations;
using System.Security;
using cCoder.ContentManagement.Services.Foundations.Storages;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Security;

namespace cCoder.ContentManagement.Services.Processings;

internal interface IAppUserProcessingService
{
    IQueryable<User> GetAppUsers(int appId);
}

internal sealed partial class AppUserProcessingService(
    IAppService appService)
        : IAppUserProcessingService
{
    public IQueryable<User> GetAppUsers(int appId) =>
        TryCatch<IQueryable<User>>(operation: () =>
    {
        ValidateAppUsersOnGet(inputs: [appId]);
        ValidateId(appId: appId, parameterName: "appId");
        App app = appService.GetApp(appId: appId);

        if (app != null)
        {
            return app.Roles
                .SelectMany(
                    selector: (Role role) =>
                        role.Users.Select(
                            selector: (UserRole userRole) =>
                                userRole.User))
                .AsQueryable();
        }

        throw new SecurityException(message: "Access Denied!");
    });

    private static void ValidateId(int appId, string parameterName) =>
        ThrowIf(
            condition: appId < 1,
            message: parameterName + " must be greater than 0.");

    private static void ThrowIf(bool condition, string message)
    {
        if (condition)
        {
            throw new ValidationException(message: message);
        }
    }
}