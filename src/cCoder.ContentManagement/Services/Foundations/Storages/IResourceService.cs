// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.CMS;

namespace cCoder.ContentManagement.Services.Foundations.Storages;

internal interface IResourceService
{
    Resource GetResource(int resourceId, bool ignoreFilters = false);

    IQueryable<Resource> GetAllResource(bool ignoreFilters = false);

    ValueTask<Resource> AddResourceAsync(Resource newResource);

    ValueTask<Resource> UpdateResourceAsync(Resource updatedResource);

    ValueTask DeleteAsync(int resourceId);
}