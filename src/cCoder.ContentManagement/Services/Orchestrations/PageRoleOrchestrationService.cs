// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.ComponentModel.DataAnnotations;
using cCoder.Data.Models.Security;
using cCoder.ContentManagement.Services.Processings;
using cCoder.ContentManagement.Models;

namespace cCoder.ContentManagement.Services.Orchestrations;

internal class PageRoleOrchestrationService(IPageRoleProcessingService processingService, IPageRoleEventProcessingService eventService) : IPageRoleOrchestrationService
{
    public IQueryable<PageRole> GetAll(bool ignoreFilters = false) =>
        processingService.GetAll(ignoreFilters: ignoreFilters);

    public async ValueTask<PageRole> AddAsync(PageRole entity)
    {
        ValidatePageRole(pageRole: entity, parameterName: "entity");
        PageRole result = await processingService.AddAsync(entity: entity);
        await eventService.RaisePageRoleAddEventAsync(entity: result);
        return result;
    }

    public async ValueTask DeleteAsync(PageRole entity)
    {
        ValidatePageRole(pageRole: entity, parameterName: "entity");
        await eventService.RaisePageRoleDeleteEventAsync(entity: entity);
        await processingService.DeleteAsync(entity: entity);
    }

    public async ValueTask<IEnumerable<Result<PageRole>>> AddOrUpdate(IEnumerable<PageRole> items)
    {
        PageRole[] pageRoles = ValidatePageRoles(pageRoles: items, parameterName: "items")
            .ToArray();

        List<Result<PageRole>> results = new();

        foreach (PageRole pageRole in pageRoles)
        {
            try
            {
                PageRole existingPageRole = processingService.GetAll(ignoreFilters: true)
                    .FirstOrDefault(predicate: existing =>
                        existing.PageId == pageRole.PageId &&
                        existing.RoleId == pageRole.RoleId);

                if (existingPageRole != null)
                {
                    results.Add(item: new Result<PageRole>
                    {
                        Id = $"{pageRole.PageId}:{pageRole.RoleId}",
                        Success = true,
                        Item = existingPageRole,
                        Message = "Already Exists"
                    });

                    continue;
                }

                PageRole result = await AddAsync(entity: pageRole);

                results.Add(item: new Result<PageRole>
                {
                    Id = $"{pageRole.PageId}:{pageRole.RoleId}",
                    Success = true,
                    Item = result,
                    Message = "Added Successfully"
                });
            }
            catch (Exception ex)
            {
                results.Add(item: new Result<PageRole>
                {
                    Id = $"{pageRole.PageId}:{pageRole.RoleId}",
                    Success = false,
                    Item = pageRole,
                    Message = ex.Message
                });
            }
        }

        return results;
    }

    public ValueTask ImportPageRolesAsync(int appId, PageRoleInfo[] items) =>
        processingService.ImportPageRolesAsync(appId: ValidateAppId(appId: appId, parameterName: "appId"), items: ValidatePageRoleInfos(pageRoleInfos: items, parameterName: "items"));

    public async ValueTask DeleteAllAsync(IEnumerable<PageRole> items)
    {
        PageRole[] pageRoles = ValidatePageRoles(pageRoles: items, parameterName: "items")
            .ToArray();

        foreach (PageRole pageRole in pageRoles)
        {
            await DeleteAsync(entity: pageRole);
        }
    }

    private static int ValidateAppId(int appId, string parameterName)
    {
        if (appId < 1)
        {
            throw new ValidationException(message: parameterName + " must be greater than 0.");
        }

        return appId;
    }

    private static PageRole ValidatePageRole(PageRole pageRole, string parameterName)
    {
        if (pageRole == null)
        {
            throw new ValidationException(message: parameterName + " is required.");
        }

        return pageRole;
    }

    private static IEnumerable<PageRole> ValidatePageRoles(IEnumerable<PageRole> pageRoles, string parameterName)
    {
        if (pageRoles == null)
        {
            throw new ValidationException(message: parameterName + " is required.");
        }

        return pageRoles;
    }

    private static PageRoleInfo[] ValidatePageRoleInfos(PageRoleInfo[] pageRoleInfos, string parameterName)
    {
        if (pageRoleInfos == null)
        {
            throw new ValidationException(message: parameterName + " is required.");
        }

        return pageRoleInfos;
    }
}