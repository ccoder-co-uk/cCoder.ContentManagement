// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Brokers.Storages;
using cCoder.Data.Models.Security;

namespace cCoder.ContentManagement.Services.Foundations.Storages;

internal sealed partial class AppUserService(IAppUserBroker appUserBroker)
    : IAppUserService
{
    public IQueryable<User> GetAllAppUser(int appId) =>
        TryCatch<IQueryable<User>>(operation: () =>
    {
        ValidateAllAppUserOnGet(inputs: [appId]);
        ArgumentOutOfRangeException.ThrowIfLessThan(value: appId, other: 1);
        return appUserBroker.GetAllAppUser(appId: appId);
    });
}