// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.CMS;
using cCoder.ContentManagement.Models;

namespace cCoder.ContentManagement.Services.Processings;

public interface IResourceProcessingService
{
    Resource GetResource(int resourceId);

    IQueryable<Resource> GetAllResource(bool ignoreFilters = false);

    ValueTask<Resource> AddResourceAsync(Resource newResource);

    ValueTask<Resource> UpdateResourceAsync(Resource updatedResource);

    ValueTask DeleteAsync(int resourceId);

    ValueTask<IEnumerable<OperationResult<Resource>>> AddOrUpdateResourceResult(IEnumerable<Resource> newResource);

    ValueTask DeleteAllResourceAsync(IEnumerable<Resource> deletedResource);
}