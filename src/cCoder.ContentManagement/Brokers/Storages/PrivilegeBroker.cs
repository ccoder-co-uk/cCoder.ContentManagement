// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data;
using cCoder.Data.Models.Security;
using Microsoft.EntityFrameworkCore;

namespace cCoder.ContentManagement.Brokers.Storages;

public interface IPrivilegeBroker
{
    IQueryable<Privilege> GetAllPrivileges(bool ignoreFilters);
}

internal sealed class PrivilegeBroker(ICoreContextFactory coreContextFactory) : IPrivilegeBroker
{
    public IQueryable<Privilege> GetAllPrivileges(bool ignoreFilters)
    {
        CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();

        return ignoreFilters
            ? coreDataContext.Set<Privilege>()
            .IgnoreQueryFilters()
            : coreDataContext.Set<Privilege>();
    }
}