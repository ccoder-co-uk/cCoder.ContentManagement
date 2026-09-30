// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Linq;
using System.Threading.Tasks;
using cCoder.Data.Models;

namespace cCoder.ContentManagement.Services.Foundations.Storages;

internal interface ICommonObjectService
{
    CommonObject GetCommonObject(int commonObjectId, bool ignoreFilters = false);

    IQueryable<CommonObject> GetAllCommonObjects(bool ignoreFilters = false);

    ValueTask<CommonObject> AddCommonObjectAsync(
        CommonObject newCommonObject,
        string userId);

    ValueTask<CommonObject> UpdateCommonObjectAsync(
        CommonObject updatedCommonObject,
        string userId);

    ValueTask DeleteAsync(int commonObjectId);

    CommonObject[] DeserializeCommonObjects(object payload);

}