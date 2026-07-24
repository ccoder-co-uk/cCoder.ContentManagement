// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.CMS;

namespace cCoder.ContentManagement.Brokers.Storages;

public interface IResourceBroker
{
    IQueryable<Resource> GetAllResources(bool ignoreFilters);

    ValueTask<Resource> AddResourceAsync(Resource newResource);

    ValueTask<Resource> UpdateResourceAsync(Resource updatedResource);

    ValueTask<int> DeleteResourceAsync(Resource deletedResource);

    ValueTask DeleteAllResourcesAsync(IEnumerable<Resource> deletedResource);
}