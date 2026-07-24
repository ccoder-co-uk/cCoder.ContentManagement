// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.CMS;
using cCoder.ContentManagement.Models;

namespace cCoder.ContentManagement.Services.Orchestrations;

public interface IResourceOrchestrationService
{
    Resource GetResource(int resourceId);

    IQueryable<Resource> GetAllResource(bool ignoreFilters = false);

    ValueTask<Resource> AddResourceAsync(Resource newResource);

    ValueTask<Resource> UpdateResourceAsync(Resource updatedResource);

    ValueTask DeleteAsync(int resourceId);

    ValueTask DeleteByAppIdAsync(int appId);

    ValueTask<IEnumerable<Result<Resource>>> AddOrUpdateResourceResult(IEnumerable<Resource> newResource);

    ValueTask ImportResourcesAsync(int appId, Resource[] items);

    ValueTask DeleteAllResourceAsync(IEnumerable<Resource> deletedResource);
}