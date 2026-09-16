// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Models;
using cCoder.Data.Models;

namespace cCoder.ContentManagement.Services.Coordinations;

public interface ICommonObjectCoordinationService
{
    CommonObject GetCommonObject(int commonObjectId);
    IQueryable<CommonObject> GetAllCommonObject(bool ignoreFilters = false);
    CommonObject[] DeserializeCommonObjects(object payload);
    ValueTask<IEnumerable<OperationResult<CommonObject>>> AddAllCommonObjectsAsync(CommonObject[] newCommonObjects);
    ValueTask<CommonObject> UpdateCommonObjectAsync(CommonObject updatedCommonObject);
    ValueTask DeleteAsync(int commonObjectId);
    ValueTask<IEnumerable<OperationResult<CommonObject>>> AddOrUpdateCommonObjectResult(IEnumerable<CommonObject> newCommonObject);
    ValueTask DeleteAllCommonObjectAsync(IEnumerable<CommonObject> deletedCommonObject);
    IEnumerable<CommonObject> LatestCommonObject(string type);
}