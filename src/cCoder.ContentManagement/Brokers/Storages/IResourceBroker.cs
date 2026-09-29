// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using cCoder.Data.Models.CMS;

namespace cCoder.ContentManagement.Brokers.Storages;

public interface IResourceBroker
{
    IQueryable<Resource> GetAllResources();

    IQueryable<Resource> GetAllResourcesIgnoringFilters();

    ValueTask<Resource> AddResourceAsync(Resource newResource);

    ValueTask<Resource> UpdateResourceAsync(Resource updatedResource);

    ValueTask<int> DeleteResourceAsync(Resource deletedResource);

    ValueTask DeleteAllResourcesAsync(IEnumerable<Resource> deletedResource);
}