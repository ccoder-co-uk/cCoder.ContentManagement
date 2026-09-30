// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Models.Results;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using cCoder.Data.Models.CMS;

namespace cCoder.ContentManagement.Services.Orchestrations;

public interface IResourceOrchestrationService
{
    Resource GetResource(int resourceId);
    IQueryable<Resource> GetAllResources(bool ignoreFilters = false);
    ValueTask<Resource> AddResourceAsync(Resource newResource);
    ValueTask<Resource> UpdateResourceAsync(Resource updatedResource);
    ValueTask DeleteAsync(int resourceId);
    ValueTask DeleteByAppIdAsync(int appId);
    ValueTask<IEnumerable<OperationResult<Resource>>> AddOrUpdateResourceResult(IEnumerable<Resource> newResource);
    ValueTask ImportResourcesAsync(int appId, Resource[] items);
    ValueTask DeleteAllResourceAsync(IEnumerable<Resource> deletedResource);
}