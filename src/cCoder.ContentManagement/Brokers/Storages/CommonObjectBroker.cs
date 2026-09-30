// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using cCoder.Data.Exposures;
using cCoder.Data.Models;

namespace cCoder.ContentManagement.Brokers.Storages;

internal sealed class CommonObjectBroker(
    ICommonObjectCacheManager commonObjectCacheManager) : ICommonObjectBroker
{
    public IQueryable<CommonObject> GetAllCommonObjects() =>
        commonObjectCacheManager.Get(
            fromCache: false,
            ignoreFilters: false);

    public IQueryable<CommonObject> GetAllCommonObjectsIgnoringFilters() =>
        commonObjectCacheManager.Get(
            fromCache: false,
            ignoreFilters: true);

    public CommonObject[] GetLatestCommonObjectsPaged(int pageSize = 500) =>
        commonObjectCacheManager.Get(
                fromCache: true,
                ignoreFilters: true)
            .GroupBy(keySelector: commonObject => new
            {
                commonObject.Name,
                Culture = commonObject.Culture ?? string.Empty,
                commonObject.Key,
                commonObject.Type
            })
            .Select(
                selector: group => group
                    .OrderByDescending(
                        keySelector: version => version.Version)
                    .First())
            .ToArray();

    public void RefreshCommonObjects() =>
        commonObjectCacheManager.Refresh();

    public ValueTask<CommonObject> AddCommonObjectAsync(
        CommonObject newCommonObject) =>
        commonObjectCacheManager.AddAsync(
            newCommonObject: newCommonObject);

    public ValueTask<CommonObject> UpdateCommonObjectAsync(
        CommonObject updatedCommonObject) =>
        commonObjectCacheManager.UpdateAsync(
            updatedCommonObject: updatedCommonObject);

    public ValueTask<int> DeleteCommonObjectAsync(
        CommonObject deletedCommonObject) =>
        commonObjectCacheManager.DeleteAsync(
            deletedCommonObject: deletedCommonObject);

    public ValueTask DeleteAllCommonObjectsAsync(
        IEnumerable<CommonObject> deletedCommonObject) =>
        commonObjectCacheManager.DeleteAllAsync(
            deletedCommonObjects: deletedCommonObject);

    public int? GetAppId(CommonObject commonObject) =>
        null;
}