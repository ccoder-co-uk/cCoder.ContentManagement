// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Models.Results;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using cCoder.Data.Models;

namespace cCoder.ContentManagement.Services.Processings;

internal interface ICommonObjectProcessingService
{
    CommonObject GetCommonObject(int commonObjectId);

    IQueryable<CommonObject> GetAllCommonObject(bool ignoreFilters = false);

    CommonObject[] DeserializeCommonObjects(object payload);

    ValueTask<CommonObject> AddCommonObjectAsync(CommonObject newCommonObject, string userId);

    ValueTask<CommonObject> UpdateCommonObjectAsync(CommonObject updatedCommonObject, string userId);

    ValueTask DeleteAsync(int commonObjectId);

    ValueTask<IEnumerable<OperationResult<CommonObject>>> AddOrUpdateCommonObjectResult(
        IEnumerable<CommonObject> newCommonObject,
        string userId);

    ValueTask DeleteAllCommonObjectAsync(IEnumerable<CommonObject> deletedCommonObject);

    ValueTask<IEnumerable<OperationResult<CommonObject>>> AddAllCommonObjectsAsync(
        CommonObject[] newCommonObjects,
        IEnumerable<CommonObject> latestCommonObjects,
        string userId);
}