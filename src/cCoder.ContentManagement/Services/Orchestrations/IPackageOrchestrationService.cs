// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Packaging;
using cCoder.ContentManagement.Models;

namespace cCoder.ContentManagement.Services.Orchestrations;

public interface IPackageOrchestrationService
{
    Package[] ExportPagackages(int appId, string[] packageNames);

    ValueTask ImportPackageAsync(int appId, Package package);

    Package Get(Guid id);

    IQueryable<Package> GetAll(bool ignoreFilters = false);

    ValueTask<Package> AddAsync(Package entity);

    ValueTask<Package> UpdateAsync(Package entity);

    ValueTask DeleteAsync(Guid id);

    ValueTask<IEnumerable<Result<Package>>> AddOrUpdate(IEnumerable<Package> items);

    ValueTask DeleteAllAsync(IEnumerable<Package> items);
}