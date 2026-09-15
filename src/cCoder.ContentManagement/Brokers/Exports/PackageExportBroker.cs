// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Security;
using Microsoft.EntityFrameworkCore;

namespace cCoder.ContentManagement.Brokers.Exports;

internal sealed class PackageExportBroker(ICoreContextFactory coreContextFactory)
    : IPackageExportBroker
{
    public IQueryable<Role> GetRoles() =>
        CreateContext().Roles.IgnoreQueryFilters();

    public IQueryable<Layout> GetLayouts() =>
        CreateContext().Layouts.IgnoreQueryFilters();

    public IQueryable<Template> GetTemplates() =>
        CreateContext().Templates.IgnoreQueryFilters();

    public IQueryable<Component> GetComponents() =>
        CreateContext().Components.IgnoreQueryFilters();

    public IQueryable<Script> GetScripts() =>
        CreateContext().Scripts.IgnoreQueryFilters();

    public IQueryable<Resource> GetResources() =>
        CreateContext().Resources.IgnoreQueryFilters();

    public IQueryable<Page> GetPages() =>
        CreateContext().Pages.IgnoreQueryFilters();

    private CoreDataContext CreateContext() =>
        coreContextFactory.CreateCoreContext();
}