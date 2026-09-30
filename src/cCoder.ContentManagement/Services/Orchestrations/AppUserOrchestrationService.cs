// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System;
using System.Linq;
using cCoder.ContentManagement.Models;
using cCoder.ContentManagement.Services.Foundations.Storages;
using cCoder.Data.Models.Security;

namespace cCoder.ContentManagement.Services.Orchestrations;

internal sealed partial class AppUserOrchestrationService(
    IAppService appService,
    IAppUserService appUserService) : IAppUserOrchestrationService
{
    public IQueryable<User> GetAllUsers(int appId) =>
        TryCatch<IQueryable<User>>(operation: () =>
    {
        ValidateAllUsersOnGet(inputs: [appId]);
        ArgumentOutOfRangeException.ThrowIfLessThan(value: appId, other: 1);

        AppOperation appOperation = appService.GetVisibleAppAppOperation(
            appOperation: new AppOperation { AppId = appId });

        if (appOperation.App == null)
        {
            throw new System.Security.SecurityException(message: "Access Denied!");
        }

        return appUserService.GetAllUsers(appId: appId);
    });
}