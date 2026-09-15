// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models;

namespace cCoder.ContentManagement.Services.Foundations.Storages;

internal interface ICommonObjectService
{
    CommonObject GetCommonObject(int commonObjectId, bool ignoreFilters = false);

    IQueryable<CommonObject> GetAllCommonObject(bool ignoreFilters = false);

    ValueTask<CommonObject> AddCommonObjectAsync(
        CommonObject newCommonObject,
        string userId);

    ValueTask<CommonObject> UpdateCommonObjectAsync(
        CommonObject updatedCommonObject,
        string userId);

    ValueTask DeleteAsync(int commonObjectId);

    CommonObject[] DeserializeCommonObjects(object payload);

}