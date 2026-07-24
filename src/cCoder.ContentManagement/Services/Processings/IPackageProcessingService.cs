// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Packaging;
using cCoder.ContentManagement.Models;

namespace cCoder.ContentManagement.Services.Processings;

public interface IPackageProcessingService
{
    Package ExportPackage(int appId, string packageName);

    Package[] ExportPackages(int appId, string[] packageNames);

    Package Get(Guid id);

    IQueryable<Package> GetAll(bool ignoreFilters = false);

    ValueTask<Package> AddAsync(Package entity);

    ValueTask<Package> UpdateAsync(Package entity);

    ValueTask DeleteAsync(Guid id);

    ValueTask<IEnumerable<Result<Package>>> AddOrUpdate(IEnumerable<Package> items);

    ValueTask DeleteAllAsync(IEnumerable<Package> items);
}