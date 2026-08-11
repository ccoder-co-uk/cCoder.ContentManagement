// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models;
using cCoder.ContentManagement.Models;

namespace cCoder.ContentManagement.Services.Processings;

internal interface ICommonObjectProcessingService
{
    CommonObject GetCommonObject(int commonObjectId);

    IQueryable<CommonObject> GetAllCommonObject(bool ignoreFilters = false);

    ValueTask<CommonObject> AddCommonObjectAsync(CommonObject newCommonObject);

    ValueTask<CommonObject> UpdateCommonObjectAsync(CommonObject updatedCommonObject);

    ValueTask DeleteAsync(int commonObjectId);

    ValueTask<IEnumerable<OperationResult<CommonObject>>> AddOrUpdateCommonObjectResult(IEnumerable<CommonObject> newCommonObject);

    ValueTask DeleteAllCommonObjectAsync(IEnumerable<CommonObject> deletedCommonObject);

    IEnumerable<CommonObject> LatestCommonObject(string type);

    ValueTask<IEnumerable<OperationResult<CommonObject>>> AddAllCommonObjectsAsync(
        CommonObject[] newCommonObjects);
}