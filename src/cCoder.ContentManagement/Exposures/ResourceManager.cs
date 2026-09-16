// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Models;
using cCoder.ContentManagement.Services.Orchestrations;
using cCoder.Data.Models.CMS;

namespace cCoder.ContentManagement.Exposures;

internal sealed class ResourceManager(IResourceOrchestrationService service) : IResourceManager
{
    public Resource GetResource(int resourceId) =>
        service.GetResource(resourceId: resourceId);

    public IQueryable<Resource> GetAllResource(bool ignoreFilters = false) =>
        service.GetAllResource(ignoreFilters: ignoreFilters);

    public ValueTask<Resource> AddResourceAsync(Resource newResource) =>
        service.AddResourceAsync(newResource: newResource);

    public ValueTask<Resource> UpdateResourceAsync(Resource updatedResource) =>
        service.UpdateResourceAsync(updatedResource: updatedResource);

    public ValueTask DeleteAsync(int resourceId) =>
        service.DeleteAsync(resourceId: resourceId);

    public ValueTask DeleteByAppIdAsync(int appId) =>
        service.DeleteByAppIdAsync(appId: appId);

    public ValueTask<IEnumerable<OperationResult<Resource>>> AddOrUpdateResourceResult(
        IEnumerable<Resource> newResource) =>
        service.AddOrUpdateResourceResult(newResource: newResource);

    public ValueTask ImportResourcesAsync(int appId, Resource[] items) =>
        service.ImportResourcesAsync(appId: appId, items: items);

    public ValueTask DeleteAllResourceAsync(IEnumerable<Resource> deletedResource) =>
        service.DeleteAllResourceAsync(deletedResource: deletedResource);
}