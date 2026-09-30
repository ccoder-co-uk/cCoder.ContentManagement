// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Models.Results;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using cCoder.CodeAnalysis.Exposures;
using cCoder.ContentManagement.Services.Orchestrations;
using cCoder.Data.Models;

namespace cCoder.ContentManagement.Exposures;

internal sealed class CommonObjectManager(
    ICommonObjectOrchestrationService commonObjectOrchestrationService)
        : ICommonObjectManager, ICompositionExposure
{
    public CommonObject GetCommonObject(int commonObjectId) =>
        commonObjectOrchestrationService.GetCommonObject(
            commonObjectId: commonObjectId);

    public IQueryable<CommonObject> GetAllCommonObjects(
        bool ignoreFilters = false) =>
        commonObjectOrchestrationService.GetAllCommonObjects(
            ignoreFilters: ignoreFilters);

    public CommonObject[] DeserializeCommonObjects(object payload) =>
        commonObjectOrchestrationService.DeserializeCommonObjects(
            payload: payload);

    public ValueTask<IEnumerable<OperationResult<CommonObject>>>
        AddAllCommonObjectsAsync(CommonObject[] newCommonObjects) =>
        commonObjectOrchestrationService.AddAllCommonObjectsAsync(
            newCommonObjects: newCommonObjects);

    public ValueTask<CommonObject> UpdateCommonObjectAsync(
        CommonObject updatedCommonObject) =>
        commonObjectOrchestrationService.UpdateCommonObjectAsync(
            updatedCommonObject: updatedCommonObject);

    public ValueTask DeleteAsync(int commonObjectId) =>
        commonObjectOrchestrationService.DeleteAsync(
            commonObjectId: commonObjectId);

    public ValueTask<IEnumerable<OperationResult<CommonObject>>>
        AddOrUpdateCommonObjectResult(
            IEnumerable<CommonObject> newCommonObject) =>
        commonObjectOrchestrationService.AddOrUpdateCommonObjectResult(
            newCommonObject: newCommonObject);

    public ValueTask DeleteAllCommonObjectAsync(
        IEnumerable<CommonObject> deletedCommonObject) =>
        commonObjectOrchestrationService.DeleteAllCommonObjectAsync(
            deletedCommonObject: deletedCommonObject);

    public IEnumerable<CommonObject> LatestCommonObjects(string type) =>
        commonObjectOrchestrationService.LatestCommonObjects(type: type);
}