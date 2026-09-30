// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System;
using System.Linq;
using cCoder.ContentManagement.Brokers.Storages;
using cCoder.Data.Models.Security;

namespace cCoder.ContentManagement.Services.Foundations.Storages;

internal sealed partial class AppUserService(IAppUserBroker appUserBroker)
    : IAppUserService
{
    public IQueryable<User> GetAllUsers(int appId) =>
        TryCatch<IQueryable<User>>(operation: () =>
    {
        ValidateAllUsersOnGet(inputs: [appId]);
        ArgumentOutOfRangeException.ThrowIfLessThan(value: appId, other: 1);
        return appUserBroker.GetAllUsers(appId: appId);
    });
}