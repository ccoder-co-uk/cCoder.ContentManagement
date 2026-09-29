// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using cCoder.Data;
using cCoder.Data.Models.Security;

namespace cCoder.ContentManagement.Brokers.Storages;

internal sealed class AppUserBroker(ICoreContextFactory coreContextFactory)
    : IAppUserBroker
{
    public IQueryable<User> GetAllAppUser(int appId)
    {
        CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();

        return coreDataContext.UserRoles
            .Where(predicate: userRole => userRole.Role.AppId == appId)
            .Select(selector: userRole => userRole.User)
            .Distinct();
    }
}