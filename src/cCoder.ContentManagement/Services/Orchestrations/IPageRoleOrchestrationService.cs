// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Models;
using cCoder.Data.Models.Security;

namespace cCoder.ContentManagement.Services.Orchestrations;

public interface IPageRoleOrchestrationService
{
    IQueryable<PageRole> GetAll(bool ignoreFilters = false);

    ValueTask<PageRole> AddAsync(PageRole entity);

    ValueTask DeleteAsync(PageRole entity);

    ValueTask<IEnumerable<Result<PageRole>>> AddOrUpdate(IEnumerable<PageRole> items);

    ValueTask ImportPageRolesAsync(int appId, PageRoleInfo[] items);

    ValueTask DeleteAllAsync(IEnumerable<PageRole> items);
}