// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.ComponentModel.DataAnnotations;
using cCoder.Data.Models.Security;
using cCoder.ContentManagement.Services.Processings;
using cCoder.ContentManagement.Models;

namespace cCoder.ContentManagement.Services.Orchestrations;

internal partial class PageRoleOrchestrationService(IPageRoleProcessingService processingService, IPageRoleEventProcessingService eventService) : IPageRoleOrchestrationService
{
    public IQueryable<PageRole> GetAllPageRole(bool ignoreFilters = false) =>
        TryCatch<IQueryable<PageRole>>(operation: () =>
    {
        ValidateAllPageRoleOnGet(inputs: [ignoreFilters]);
        return processingService.GetAllPageRole(ignoreFilters: ignoreFilters);
    });

    public ValueTask<PageRole> AddPageRoleAsync(PageRole newPageRole) =>
        TryCatch<PageRole>(operation: async () =>
    {
        ValidatePageRoleOnAdd(inputs: [newPageRole]);
        ValidatePageRole(pageRole: newPageRole, parameterName: "entity");
        PageRole result = await processingService.AddPageRoleAsync(newPageRole: newPageRole);
        await eventService.RaisePageRoleAddEventAsync(entity: result);
        return result;

    }, isValueTask: true);

    public ValueTask DeletePageRoleAsync(PageRole deletedPageRole) =>
        TryCatch(operation: async () =>
    {
        ValidatePageRoleOnDelete(inputs: [deletedPageRole]);
        ValidatePageRole(pageRole: deletedPageRole, parameterName: "entity");
        await eventService.RaisePageRoleDeleteEventAsync(entity: deletedPageRole);
        await processingService.DeletePageRoleAsync(deletedPageRole: deletedPageRole);

    }, isValueTask: true);

    public ValueTask<IEnumerable<OperationResult<PageRole>>> AddOrUpdatePageRoleResult(IEnumerable<PageRole> newPageRole) =>
        TryCatch<IEnumerable<OperationResult<PageRole>>>(operation: async () =>
    {
        ValidateOrUpdatePageRoleResultOnAdd(inputs: [newPageRole]);

        PageRole[] pageRoles = ValidatePageRoles(pageRoles: newPageRole, parameterName: "items")
            .ToArray();

        List<OperationResult<PageRole>> results = new();

        foreach (PageRole pageRole in pageRoles)
        {
            try
            {
                PageRole existingPageRole = processingService.GetAllPageRole(ignoreFilters: true)
                    .FirstOrDefault(predicate: existing =>
                        existing.PageId == pageRole.PageId &&
                        existing.RoleId == pageRole.RoleId);

                if (existingPageRole != null)
                {
                    results.Add(item: new OperationResult<PageRole>
                    {
                        Id = $"{pageRole.PageId}:{pageRole.RoleId}",
                        Success = true,
                        Item = existingPageRole,
                        Message = "Already Exists"
                    });

                    continue;
                }

                PageRole result = await ExecuteAddPageRoleAsync(newPageRole: pageRole);

                results.Add(item: new OperationResult<PageRole>
                {
                    Id = $"{pageRole.PageId}:{pageRole.RoleId}",
                    Success = true,
                    Item = result,
                    Message = "Added Successfully"
                });
            }
            catch (Exception ex)
            {
                results.Add(item: new OperationResult<PageRole>
                {
                    Id = $"{pageRole.PageId}:{pageRole.RoleId}",
                    Success = false,
                    Item = pageRole,
                    Message = ex.Message
                });
            }
        }

        return results;

    }, isValueTask: true);

    public ValueTask DeleteAllPageRoleAsync(IEnumerable<PageRole> deletedPageRole) =>
        TryCatch(operation: async () =>
    {
        ValidateAllPageRoleOnDelete(inputs: [deletedPageRole]);

        PageRole[] pageRoles = ValidatePageRoles(pageRoles: deletedPageRole, parameterName: "items")
            .ToArray();

        foreach (PageRole pageRole in pageRoles)
        {
            await ExecuteDeletePageRoleAsync(deletedPageRole: pageRole);
        }

    }, isValueTask: true);

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

    private async ValueTask<PageRole> ExecuteAddPageRoleAsync(PageRole newPageRole)
    {
        ValidatePageRole(pageRole: newPageRole, parameterName: "entity");
        PageRole result = await processingService.AddPageRoleAsync(newPageRole: newPageRole);
        await eventService.RaisePageRoleAddEventAsync(entity: result);
        return result;
    }

    private async ValueTask ExecuteDeletePageRoleAsync(PageRole deletedPageRole)
    {
        ValidatePageRole(pageRole: deletedPageRole, parameterName: "entity");
        await eventService.RaisePageRoleDeleteEventAsync(entity: deletedPageRole);
        await processingService.DeletePageRoleAsync(deletedPageRole: deletedPageRole);
    }
}