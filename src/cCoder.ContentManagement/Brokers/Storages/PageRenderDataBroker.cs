// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Models;
using cCoder.Data;
using cCoder.Data.Models.CMS;
using Microsoft.EntityFrameworkCore;

namespace cCoder.ContentManagement.Brokers.Storages;

internal sealed class PageRenderDataBroker(
    ICoreContextFactory coreContextFactory) : IPageRenderDataBroker
{
    public async ValueTask<PageRenderData> GetPageRenderDataAsync(int pageId)
    {
        Page page = await GetPageAsync(pageId: pageId);
        int appId = page?.AppId ?? 0;

        Task<Layout[]> layouts = GetLayoutsAsync(appId: appId);
        Task<Template[]> templates = GetTemplatesAsync(appId: appId);
        Task<Resource[]> resources = GetResourcesAsync(appId: appId);
        Task<Component[]> components = GetComponentsAsync(appId: appId);
        Task<Script[]> scripts = GetScriptsAsync(appId: appId);
        Task<Page[]> pages = GetPagesAsync(appId: appId);

        await Task.WhenAll(
            tasks:
            [
                layouts,
                templates,
                resources,
                components,
                scripts,
                pages
            ]);

        return new PageRenderData
        {
            Page = page,
            Layouts = await layouts,
            Templates = await templates,
            Resources = await resources,
            Components = await components,
            Scripts = await scripts,
            Pages = await pages
        };
    }

    private async Task<Page> GetPageAsync(int pageId)
    {
        await using CoreDataContext coreDataContext =
            coreContextFactory.CreateCoreContext();

        return await coreDataContext.Pages
            .IgnoreQueryFilters()
            .AsNoTracking()
            .Include(navigationPropertyPath: page => page.PageInfo)
            .Include(navigationPropertyPath: page => page.Contents)
            .Include(navigationPropertyPath: page => page.Roles)
            .Include(navigationPropertyPath: page => page.App)
            .SingleOrDefaultAsync(predicate: page => page.Id == pageId);
    }

    private async Task<Layout[]> GetLayoutsAsync(int appId)
    {
        await using CoreDataContext coreDataContext =
            coreContextFactory.CreateCoreContext();

        return await coreDataContext.Layouts
            .IgnoreQueryFilters()
            .AsNoTracking()
            .Where(predicate: layout => layout.AppId == appId)
            .ToArrayAsync();
    }

    private async Task<Template[]> GetTemplatesAsync(int appId)
    {
        await using CoreDataContext coreDataContext =
            coreContextFactory.CreateCoreContext();

        return await coreDataContext.Templates
            .IgnoreQueryFilters()
            .AsNoTracking()
            .Where(predicate: template => template.AppId == appId)
            .ToArrayAsync();
    }

    private async Task<Resource[]> GetResourcesAsync(int appId)
    {
        await using CoreDataContext coreDataContext =
            coreContextFactory.CreateCoreContext();

        return await coreDataContext.Resources
            .IgnoreQueryFilters()
            .AsNoTracking()
            .Where(predicate: resource => resource.AppId == appId)
            .ToArrayAsync();
    }

    private async Task<Component[]> GetComponentsAsync(int appId)
    {
        await using CoreDataContext coreDataContext =
            coreContextFactory.CreateCoreContext();

        return await coreDataContext.Components
            .IgnoreQueryFilters()
            .AsNoTracking()
            .Where(predicate: component => component.AppId == appId)
            .ToArrayAsync();
    }

    private async Task<Script[]> GetScriptsAsync(int appId)
    {
        await using CoreDataContext coreDataContext =
            coreContextFactory.CreateCoreContext();

        return await coreDataContext.Scripts
            .IgnoreQueryFilters()
            .AsNoTracking()
            .Where(predicate: script => script.AppId == appId)
            .ToArrayAsync();
    }

    private async Task<Page[]> GetPagesAsync(int appId)
    {
        await using CoreDataContext coreDataContext =
            coreContextFactory.CreateCoreContext();

        return await coreDataContext.Pages
            .IgnoreQueryFilters()
            .AsNoTracking()
            .Include(navigationPropertyPath: page => page.PageInfo)
            .Where(predicate: page => page.AppId == appId)
            .ToArrayAsync();
    }
}