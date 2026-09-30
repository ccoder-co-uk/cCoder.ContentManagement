// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Models.Results;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using cCoder.Data.Models.CMS;

namespace cCoder.ContentManagement.Services.Processings;

internal interface IResourceProcessingService
{
    Resource GetResource(int resourceId);

    IQueryable<Resource> GetAllResources(bool ignoreFilters = false);

    ValueTask<Resource> AddResourceAsync(Resource newResource);

    ValueTask<Resource> UpdateResourceAsync(Resource updatedResource);

    ValueTask DeleteAsync(int resourceId);

    ValueTask<IEnumerable<OperationResult<Resource>>> AddOrUpdateResourceResult(IEnumerable<Resource> newResource);

    ValueTask DeleteAllResourceAsync(IEnumerable<Resource> deletedResource);
}