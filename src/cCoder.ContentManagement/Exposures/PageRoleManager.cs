// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Models;
using cCoder.ContentManagement.Services.Orchestrations;
using cCoder.Data.Models.Security;

namespace cCoder.ContentManagement.Exposures;

internal sealed class PageRoleManager(IPageRoleOrchestrationService service) : IPageRoleManager
{
    public IQueryable<PageRole> GetAllPageRole(bool ignoreFilters = false) =>
        service.GetAllPageRole(ignoreFilters: ignoreFilters);

    public ValueTask<PageRole> AddPageRoleAsync(PageRole newPageRole) =>
        service.AddPageRoleAsync(newPageRole: newPageRole);

    public ValueTask DeletePageRoleAsync(PageRole deletedPageRole) =>
        service.DeletePageRoleAsync(deletedPageRole: deletedPageRole);

    public ValueTask<IEnumerable<OperationResult<PageRole>>> AddOrUpdatePageRoleResult(
        IEnumerable<PageRole> newPageRole) =>
        service.AddOrUpdatePageRoleResult(newPageRole: newPageRole);

    public ValueTask DeleteAllPageRoleAsync(IEnumerable<PageRole> deletedPageRole) =>
        service.DeleteAllPageRoleAsync(deletedPageRole: deletedPageRole);
}