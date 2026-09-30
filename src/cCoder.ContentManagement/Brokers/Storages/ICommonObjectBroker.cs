// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using cCoder.Data.Models;

namespace cCoder.ContentManagement.Brokers.Storages;

public interface ICommonObjectBroker
{
    IQueryable<CommonObject> GetAllCommonObjects();

    IQueryable<CommonObject> GetAllCommonObjectsIgnoringFilters();

    CommonObject[] GetLatestCommonObjectsPaged(int pageSize = 500);

    void RefreshCommonObjects();

    ValueTask<CommonObject> AddCommonObjectAsync(CommonObject newCommonObject);

    ValueTask<CommonObject> UpdateCommonObjectAsync(CommonObject updatedCommonObject);

    ValueTask<int> DeleteCommonObjectAsync(CommonObject deletedCommonObject);

    ValueTask DeleteAllCommonObjectsAsync(IEnumerable<CommonObject> deletedCommonObject);

    int? GetAppId(CommonObject entity);
}