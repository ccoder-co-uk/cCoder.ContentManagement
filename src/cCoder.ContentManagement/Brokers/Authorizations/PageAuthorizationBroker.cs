// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data;
using Microsoft.EntityFrameworkCore;

namespace cCoder.ContentManagement.Brokers.Authorizations;

internal sealed class PageAuthorizationBroker(
    ICoreContextFactory coreContextFactory) : IPageAuthorizationBroker
{
    public async ValueTask<int?> GetAuthorizedPageIdAsync(
        string domain,
        string path)
    {
        await using CoreDataContext context =
            coreContextFactory.CreateCoreContext();

        return await context.Pages
            .Where(predicate: page =>
                page.App.Domain == domain
                && page.Path == path)
            .Select(selector: page => (int?)page.Id)
            .SingleOrDefaultAsync();
    }

    public async ValueTask<int?> GetPageIdIgnoringFiltersAsync(
        string domain,
        string path)
    {
        await using CoreDataContext context =
            coreContextFactory.CreateCoreContext();

        return await context.Pages
            .IgnoreQueryFilters()
            .Where(predicate: page =>
                page.App.Domain == domain
                && page.Path == path)
            .Select(selector: page => (int?)page.Id)
            .SingleOrDefaultAsync();
    }
}