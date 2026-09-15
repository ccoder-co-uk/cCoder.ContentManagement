// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Security;

namespace cCoder.ContentManagement.Brokers.Exports;

internal interface IPackageExportBroker
{
    IQueryable<Role> GetRoles();

    IQueryable<Layout> GetLayouts();

    IQueryable<Template> GetTemplates();

    IQueryable<Component> GetComponents();

    IQueryable<Script> GetScripts();

    IQueryable<Resource> GetResources();

    IQueryable<Page> GetPages();
}