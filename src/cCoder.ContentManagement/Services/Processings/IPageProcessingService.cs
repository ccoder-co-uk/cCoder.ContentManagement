// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.CMS;
using cCoder.ContentManagement.Models;

namespace cCoder.ContentManagement.Services.Processings;

public interface IPageProcessingService
{
    Page Get(int id);

    IQueryable<Page> GetAll(bool ignoreFilters = false);

    ValueTask<Page> AddAsync(Page entity);

    ValueTask<Page> UpdateAsync(Page entity);

    ValueTask DeleteAsync(int id);

    ValueTask<IEnumerable<Result<Page>>> AddOrUpdate(IEnumerable<Page> items);

    ValueTask DeleteAllAsync(IEnumerable<Page> items);

    ValueTask RecomputeAllForAppAsync(int appId);

    Page GetRoot(int id);

    IEnumerable<Page> GetChildren(int id);

    string MenuFor(int id, string culture);
}