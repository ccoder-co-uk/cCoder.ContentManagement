// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.ComponentModel.DataAnnotations;
using cCoder.ContentManagement.Services.Foundations.Storages;
using cCoder.Data.Models.CMS;
using cCoder.ContentManagement.Models;

namespace cCoder.ContentManagement.Services.Processings;

internal class PageInfoProcessingService(IPageInfoService service) : IPageInfoProcessingService
{
    public PageInfo Get(int id)
    {
        ValidateId(id: id, parameterName: "id");
        return service.Get(id: id);
    }

    public IQueryable<PageInfo> GetAll(bool ignoreFilters = false) =>
        service.GetAll(ignoreFilters: ignoreFilters);

    public ValueTask<PageInfo> AddAsync(PageInfo entity)
    {
        ValidatePageInfo(pageInfo: entity, parameterName: "entity");
        return service.AddAsync(pageInfo: entity);
    }

    public ValueTask<PageInfo> UpdateAsync(PageInfo entity)
    {
        ValidatePageInfo(pageInfo: entity, parameterName: "entity");
        return service.UpdateAsync(pageInfo: entity);
    }

    public ValueTask DeleteAsync(int id)
    {
        ValidateId(id: id, parameterName: "id");
        return service.DeleteAsync(id: id);
    }

    public async ValueTask<IEnumerable<Result<PageInfo>>> AddOrUpdate(IEnumerable<PageInfo> items)
    {
        ValidatePageInfos(pageInfos: items, parameterName: "items");
        List<Result<PageInfo>> results = new List<Result<PageInfo>>();

        foreach (PageInfo item in items)
        {
            try
            {
                PageInfo savedItem = item.Id < 1 ? await AddAsync(entity: item) : await UpdateAsync(entity: item);

                results.Add(item: new Result<PageInfo>
                {
                    Success = true,
                    Item = savedItem,
                    Message = item.Id < 1 ? "Added Successfully" : "Updated Successfully"
                });
            }
            catch (Exception ex)
            {
                results.Add(item: new Result<PageInfo>
                {
                    Success = false,
                    Item = item,
                    Message = ex.Message
                });
            }
        }

        return results;
    }

    public async ValueTask DeleteAllAsync(IEnumerable<PageInfo> items)
    {
        ValidatePageInfos(pageInfos: items, parameterName: "items");

        foreach (PageInfo item in items)
        {
            await DeleteAsync(id: item.Id);
        }
    }

    private static void ValidateId(int id, string parameterName) =>
        ThrowIf(condition: id < 1, message: parameterName + " must be greater than 0.");

    private static void ValidatePageInfo(PageInfo pageInfo, string parameterName) =>
        ThrowIf(condition: pageInfo == null, message: parameterName + " is required.");

    private static void ValidatePageInfos(IEnumerable<PageInfo> pageInfos, string parameterName) =>
        ThrowIf(condition: pageInfos == null, message: parameterName + " is required.");

    private static void ThrowIf(bool condition, string message)
    {
        if (condition)
        {
            throw new ValidationException(message: message);
        }
    }
}