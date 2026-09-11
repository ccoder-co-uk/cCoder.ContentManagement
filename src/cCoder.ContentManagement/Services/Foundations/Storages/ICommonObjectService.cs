// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models;

namespace cCoder.ContentManagement.Services.Foundations.Storages;

internal interface ICommonObjectService
{
    CommonObject GetCommonObject(int commonObjectId, bool ignoreFilters = false);

    IQueryable<CommonObject> GetAllCommonObject(bool ignoreFilters = false);

    ValueTask<CommonObject> AddCommonObjectAsync(CommonObject newCommonObject);

    ValueTask<CommonObject> UpdateCommonObjectAsync(CommonObject updatedCommonObject);

    ValueTask DeleteAsync(int commonObjectId);

    CommonObject[] DeserializeCommonObjects(object payload);

    string GetCurrentUserId();

    void Authorize(int? appId, string privilege);

    bool IsAdminOfApp(int appId);

    IEnumerable<CommonObject> GetLatestSet();

    void CacheComponent(CommonObject commonObject);

    void CacheResource(CommonObject commonObject);

    void CacheScript(CommonObject commonObject);
}