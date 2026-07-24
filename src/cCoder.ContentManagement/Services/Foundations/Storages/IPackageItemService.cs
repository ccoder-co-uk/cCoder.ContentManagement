// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Packaging;

namespace cCoder.ContentManagement.Services.Foundations.Storages;

public interface IPackageItemService
{
    PackageItem Get(Guid id, bool ignoreFilters = false);

    IQueryable<PackageItem> GetAll(bool ignoreFilters = false);

    ValueTask<PackageItem> AddAsync(PackageItem packageItem);

    ValueTask<PackageItem> UpdateAsync(PackageItem packageItem);

    ValueTask DeleteAsync(Guid id);
}