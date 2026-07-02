using System.ComponentModel.DataAnnotations;
using cCoder.Data.Models.Security;
using cCoder.ContentManagement.Services.Processings;
using cCoder.ContentManagement.Models;

namespace cCoder.ContentManagement.Services.Orchestrations;

internal class PageRoleOrchestrationService(IPageRoleProcessingService processingService, IPageRoleEventProcessingService eventService) : IPageRoleOrchestrationService
{
    public IQueryable<PageRole> GetAll(bool ignoreFilters = false) =>
        processingService.GetAll(ignoreFilters);

    public async ValueTask<PageRole> AddAsync(PageRole entity)
    {
        ValidatePageRole(entity, "entity");
        PageRole result = await processingService.AddAsync(entity);
        await eventService.RaisePageRoleAddEventAsync(result);
        return result;
    }

    public async ValueTask DeleteAsync(PageRole entity)
    {
        ValidatePageRole(entity, "entity");
        await eventService.RaisePageRoleDeleteEventAsync(entity);
        await processingService.DeleteAsync(entity);
    }

    public async ValueTask<IEnumerable<Result<PageRole>>> AddOrUpdate(IEnumerable<PageRole> items)
    {
        PageRole[] pageRoles = ValidatePageRoles(items, "items").ToArray();
        List<Result<PageRole>> results = new();

        foreach (PageRole pageRole in pageRoles)
        {
            try
            {
                PageRole existingPageRole = processingService.GetAll(ignoreFilters: true)
                    .FirstOrDefault(existing =>
                        existing.PageId == pageRole.PageId &&
                        existing.RoleId == pageRole.RoleId);

                if (existingPageRole != null)
                {
                    results.Add(new Result<PageRole>
                    {
                        Id = $"{pageRole.PageId}:{pageRole.RoleId}",
                        Success = true,
                        Item = existingPageRole,
                        Message = "Already Exists"
                    });

                    continue;
                }

                PageRole result = await AddAsync(pageRole);
                results.Add(new Result<PageRole>
                {
                    Id = $"{pageRole.PageId}:{pageRole.RoleId}",
                    Success = true,
                    Item = result,
                    Message = "Added Successfully"
                });
            }
            catch (Exception ex)
            {
                results.Add(new Result<PageRole>
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
        processingService.ImportPageRolesAsync(ValidateAppId(appId, "appId"), ValidatePageRoleInfos(items, "items"));

    public async ValueTask DeleteAllAsync(IEnumerable<PageRole> items)
    {
        PageRole[] pageRoles = ValidatePageRoles(items, "items").ToArray();

        foreach (PageRole pageRole in pageRoles)
            await DeleteAsync(pageRole);
    }

    private static int ValidateAppId(int appId, string parameterName)
    {
        if (appId < 1)
            throw new ValidationException(parameterName + " must be greater than 0.");

        return appId;
    }

    private static PageRole ValidatePageRole(PageRole pageRole, string parameterName)
    {
        if (pageRole == null)
            throw new ValidationException(parameterName + " is required.");

        return pageRole;
    }

    private static IEnumerable<PageRole> ValidatePageRoles(IEnumerable<PageRole> pageRoles, string parameterName)
    {
        if (pageRoles == null)
            throw new ValidationException(parameterName + " is required.");

        return pageRoles;
    }

    private static PageRoleInfo[] ValidatePageRoleInfos(PageRoleInfo[] pageRoleInfos, string parameterName)
    {
        if (pageRoleInfos == null)
            throw new ValidationException(parameterName + " is required.");

        return pageRoleInfos;
    }
}
