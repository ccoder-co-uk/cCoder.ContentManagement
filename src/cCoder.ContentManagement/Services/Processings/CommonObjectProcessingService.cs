// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.ComponentModel.DataAnnotations;
using cCoder.ContentManagement.Models;
using cCoder.ContentManagement.Services.Foundations.Storages;
using cCoder.Data.Models;

namespace cCoder.ContentManagement.Services.Processings;

internal partial class CommonObjectProcessingService(
    ICommonObjectService service)
    : ICommonObjectProcessingService
{
    public CommonObject[] DeserializeCommonObjects(object payload) =>
        TryCatch(operation: () =>
    {
        ValidateDeserializeCommonObjects(inputs: [payload]);
        return service.DeserializeCommonObjects(payload: payload);
    });

    public CommonObject GetCommonObject(int commonObjectId) =>
        TryCatch(operation: () =>
    {
        ValidateCommonObjectOnGet(inputs: [commonObjectId]);
        ValidateId(commonObjectId: commonObjectId, parameterName: "id");
        return service.GetCommonObject(commonObjectId: commonObjectId);
    });

    public IQueryable<CommonObject> GetAllCommonObject(bool ignoreFilters = false) =>
        TryCatch(operation: () =>
    {
        ValidateAllCommonObjectOnGet(inputs: [ignoreFilters]);

        return service.GetAllCommonObject(ignoreFilters: ignoreFilters);
    });

    public ValueTask<IEnumerable<OperationResult<CommonObject>>> AddAllCommonObjectsAsync(
        CommonObject[] newCommonObjects,
        IEnumerable<CommonObject> latestCommonObjects,
        string userId) =>
        TryCatch<IEnumerable<OperationResult<CommonObject>>>(operation: async () =>
    {
        ValidateAllCommonObjectsOnAdd(
            inputs: [newCommonObjects, latestCommonObjects, userId]);

        ValidateCommonObjects(
            commonObjects: newCommonObjects,
            parameterName: "newCommonObjects");

        foreach (CommonObject commonObject in newCommonObjects)
        {
            NormalizeCulture(commonObject: commonObject);
        }

        List<CommonObject> adds = [];
        List<CommonObject> updates = [];

        foreach (string type in newCommonObjects
            .Select(selector: item => item.Type)
            .Distinct())
        {
            CommonObject[] existingSet = latestCommonObjects
                .Where(predicate: item => item.Type == type)
                .ToArray();

            foreach (CommonObject entry in newCommonObjects.Where(
                predicate: item => item.Type == type))
            {
                CommonObject matched = existingSet.FirstOrDefault(
                    predicate: existing =>
                        existing.Culture == entry.Culture
                        && existing.Name == entry.Name
                        && existing.Key == entry.Key);

                if (matched is null)
                {
                    entry.Id = 0;
                    entry.Version = 1;
                    adds.Add(item: entry);
                }
                else if (entry.CreatedOn > matched.CreatedOn
                    || entry.LastUpdated > matched.LastUpdated)
                {
                    entry.Version = matched.Version + 1;
                    updates.Add(item: entry);
                }
            }
        }

        List<OperationResult<CommonObject>> results = [];

        results.AddRange(collection:
            await ExecuteAddOrUpdateCommonObjectResult(
                commonObjects: adds,
                userId: userId));

        results.AddRange(collection:
            await ExecuteAddOrUpdateCommonObjectResult(
                commonObjects: updates,
                userId: userId));

        return results;
    }, isValueTask: true);

    public ValueTask<CommonObject> AddCommonObjectAsync(
        CommonObject newCommonObject,
        string userId) =>
        TryCatch<CommonObject>(operation: async () =>
    {
        ValidateCommonObjectOnAdd(inputs: [newCommonObject, userId]);

        return await ExecuteAddCommonObjectAsync(
            newCommonObject: newCommonObject,
            userId: userId);
    }, isValueTask: true);

    public ValueTask<CommonObject> UpdateCommonObjectAsync(
        CommonObject updatedCommonObject,
        string userId) =>
        TryCatch<CommonObject>(operation: async () =>
    {
        ValidateCommonObjectOnUpdate(inputs: [updatedCommonObject, userId]);

        return await ExecuteUpdateCommonObjectAsync(
            updatedCommonObject: updatedCommonObject,
            userId: userId);
    }, isValueTask: true);

    public ValueTask DeleteAsync(int commonObjectId) =>
        TryCatch(operation: async () =>
    {
        ValidateDeleteAsync(inputs: [commonObjectId]);

        await ExecuteDeleteAsync(commonObjectId: commonObjectId);
    }, isValueTask: true);

    public ValueTask<IEnumerable<OperationResult<CommonObject>>> AddOrUpdateCommonObjectResult(
        IEnumerable<CommonObject> newCommonObject,
        string userId) =>
        TryCatch<IEnumerable<OperationResult<CommonObject>>>(operation: async () =>
    {
        ValidateOrUpdateCommonObjectResultOnAdd(inputs: [newCommonObject, userId]);

        ValidateCommonObjects(
            commonObjects: newCommonObject,
            parameterName: "items");

        return await ExecuteAddOrUpdateCommonObjectResult(
            commonObjects: newCommonObject,
            userId: userId);
    }, isValueTask: true);

    public ValueTask DeleteAllCommonObjectAsync(
        IEnumerable<CommonObject> deletedCommonObject) =>
        TryCatch(operation: async () =>
    {
        ValidateAllCommonObjectOnDelete(inputs: [deletedCommonObject]);

        ValidateCommonObjects(
            commonObjects: deletedCommonObject,
            parameterName: "items");

        foreach (CommonObject item in deletedCommonObject)
        {
            await ExecuteDeleteAsync(commonObjectId: item.Id);
        }
    }, isValueTask: true);

    private async ValueTask<CommonObject> ExecuteAddCommonObjectAsync(
        CommonObject newCommonObject,
        string userId)
    {
        ValidateCommonObject(
            commonObject: newCommonObject,
            parameterName: "entity");

        NormalizeCulture(commonObject: newCommonObject);

        return await service.AddCommonObjectAsync(
            newCommonObject: newCommonObject,
            userId: userId);
    }

    private async ValueTask<IEnumerable<OperationResult<CommonObject>>>
        ExecuteAddOrUpdateCommonObjectResult(
            IEnumerable<CommonObject> commonObjects,
            string userId)
    {
        List<OperationResult<CommonObject>> results = [];

        foreach (CommonObject item in commonObjects)
        {
            try
            {
                bool isNew = item.Id < 1;

                CommonObject savedItem = isNew
                    ? await ExecuteAddCommonObjectAsync(
                        newCommonObject: item,
                        userId: userId)
                    : await ExecuteUpdateCommonObjectAsync(
                        updatedCommonObject: item,
                        userId: userId);

                results.Add(item: new OperationResult<CommonObject>
                {
                    Success = true,
                    Item = savedItem,
                    Message = isNew ? "Added Successfully" : "Updated Successfully"
                });
            }
            catch (Exception exception)
            {
                results.Add(item: new OperationResult<CommonObject>
                {
                    Success = false,
                    Item = item,
                    Message = exception.Message
                });
            }
        }

        return results;
    }

    private async ValueTask ExecuteDeleteAsync(int commonObjectId)
    {
        ValidateId(commonObjectId: commonObjectId, parameterName: "id");
        await service.DeleteAsync(commonObjectId: commonObjectId);
    }

    private async ValueTask<CommonObject> ExecuteUpdateCommonObjectAsync(
        CommonObject updatedCommonObject,
        string userId)
    {
        ValidateCommonObject(
            commonObject: updatedCommonObject,
            parameterName: "entity");

        NormalizeCulture(commonObject: updatedCommonObject);

        int versionCount = service.GetAllCommonObject()
            .Count(predicate: item => item.Name == updatedCommonObject.Name
                && item.Type == updatedCommonObject.Type
                && item.Culture == updatedCommonObject.Culture
                && item.Key == updatedCommonObject.Key) + 1;

        int nextStoredVersion = (service.GetAllCommonObject()
            .Where(predicate: item => item.Name == updatedCommonObject.Name
                && item.Type == updatedCommonObject.Type
                && item.Culture == updatedCommonObject.Culture
                && item.Key == updatedCommonObject.Key)
            .OrderByDescending(keySelector: item => item.Version)
            .FirstOrDefault()?.Version ?? 0) + 1;

        updatedCommonObject.Id = 0;

        updatedCommonObject.Version = Math.Max(
            val1: versionCount,
            val2: nextStoredVersion);

        updatedCommonObject.CreatedOn = DateTimeOffset.Now;
        updatedCommonObject.LastUpdated = DateTimeOffset.Now;
        updatedCommonObject.LastUpdatedBy = userId;
        updatedCommonObject.CreatedBy = userId;

        return await service.AddCommonObjectAsync(
            newCommonObject: updatedCommonObject,
            userId: userId);
    }

    private static void ValidateId(int commonObjectId, string parameterName) =>
        ThrowIf(
            condition: commonObjectId < 1,
            message: parameterName + " must be greater than 0.");

    private static void ValidateCommonObject(CommonObject commonObject, string parameterName) =>
        ThrowIf(
            condition: commonObject is null,
            message: parameterName + " is required.");

    private static void ValidateCommonObjects(
        IEnumerable<CommonObject> commonObjects,
        string parameterName) =>
        ThrowIf(
            condition: commonObjects is null,
            message: parameterName + " is required.");

    private static void NormalizeCulture(CommonObject commonObject) =>
        commonObject.Culture ??= string.Empty;

    private static void ThrowIf(bool condition, string message)
    {
        if (condition)
        {
            throw new ValidationException(message: message);
        }
    }
}