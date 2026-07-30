// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data;
using cCoder.Data.Models.Security;
using Microsoft.EntityFrameworkCore;

namespace cCoder.ContentManagement.Brokers.Storages;

public interface IPrivilegeBroker
{
    IQueryable<Privilege> GetAllPrivileges();

    IQueryable<Privilege> GetAllPrivilegesIgnoringFilters();
}

internal sealed class PrivilegeBroker(ICoreContextFactory coreContextFactory) : IPrivilegeBroker
{
    public IQueryable<Privilege> GetAllPrivileges()
    {
        CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();

        return coreDataContext.Set<Privilege>();
    }

    public IQueryable<Privilege> GetAllPrivilegesIgnoringFilters()
    {
        CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();

        return coreDataContext
            .Set<Privilege>()
            .IgnoreQueryFilters();
    }
}