// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Models.Results;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using cCoder.CodeAnalysis.Exposures;
using cCoder.ContentManagement.Services.Coordinations;
using cCoder.Data.Models;

namespace cCoder.ContentManagement.Exposures;

internal sealed class CommonObjectManager(
    ICommonObjectCoordinationService commonObjectCoordinationService)
        : ICommonObjectManager, ICompositionExposure
{
    public CommonObject GetCommonObject(int commonObjectId) =>
        commonObjectCoordinationService.GetCommonObject(
            commonObjectId: commonObjectId);

    public IQueryable<CommonObject> GetAllCommonObject(
        bool ignoreFilters = false) =>
        commonObjectCoordinationService.GetAllCommonObject(
            ignoreFilters: ignoreFilters);

    public CommonObject[] DeserializeCommonObjects(object payload) =>
        commonObjectCoordinationService.DeserializeCommonObjects(
            payload: payload);

    public ValueTask<IEnumerable<OperationResult<CommonObject>>>
        AddAllCommonObjectsAsync(CommonObject[] newCommonObjects) =>
        commonObjectCoordinationService.AddAllCommonObjectsAsync(
            newCommonObjects: newCommonObjects);

    public ValueTask<CommonObject> UpdateCommonObjectAsync(
        CommonObject updatedCommonObject) =>
        commonObjectCoordinationService.UpdateCommonObjectAsync(
            updatedCommonObject: updatedCommonObject);

    public ValueTask DeleteAsync(int commonObjectId) =>
        commonObjectCoordinationService.DeleteAsync(
            commonObjectId: commonObjectId);

    public ValueTask<IEnumerable<OperationResult<CommonObject>>>
        AddOrUpdateCommonObjectResult(
            IEnumerable<CommonObject> newCommonObject) =>
        commonObjectCoordinationService.AddOrUpdateCommonObjectResult(
            newCommonObject: newCommonObject);

    public ValueTask DeleteAllCommonObjectAsync(
        IEnumerable<CommonObject> deletedCommonObject) =>
        commonObjectCoordinationService.DeleteAllCommonObjectAsync(
            deletedCommonObject: deletedCommonObject);

    public IEnumerable<CommonObject> LatestCommonObject(string type) =>
        commonObjectCoordinationService.LatestCommonObject(type: type);
}