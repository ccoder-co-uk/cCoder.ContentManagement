using System.ComponentModel.DataAnnotations;
using cCoder.ContentManagement.Services.Orchestrations;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Security;

namespace cCoder.ContentManagement.Services.Coordinations;

internal class PageCoordinationService(
    IPageInfoOrchestrationService pageInfoOrchestrationService,
    IContentOrchestrationService contentOrchestrationService,
    IPageRoleOrchestrationService pageRoleOrchestrationService,
    IPageOrchestrationService pageOrchestrationService) : IPageCoordinationService
{
    public async ValueTask HandlePageAddAsync(Page page)
    {
        ValidatePage(page, "page");

        if (page.PageInfo != null)
        {
            PageInfo[] pageInfos = page.PageInfo.Select((PageInfo pageInfo) => new PageInfo
            {
                Id = pageInfo.Id,
                PageId = page.Id,
                CultureId = pageInfo.CultureId,
                Title = pageInfo.Title,
                Description = pageInfo.Description,
                Keywords = pageInfo.Keywords
            }).ToArray();

            await pageInfoOrchestrationService.AddOrUpdate(pageInfos);
        }

        if (page.Contents != null)
        {
            Content[] contents = page.Contents
                .Select(content =>
                {
                    content.PageId = page.Id;
                    return content;
                })
                .ToArray();

            await contentOrchestrationService.AddOrUpdate(contents);
        }

        if (page.Roles != null)
        {
            PageRole[] pageRoles = page.Roles.Select(pageRole => new PageRole
            {
                PageId = page.Id,
                RoleId = pageRole.RoleId
            }).ToArray();

            await pageRoleOrchestrationService.AddOrUpdate(pageRoles);
        }
    }

    public async ValueTask HandlePageUpdateAsync(Page page)
    {
        ValidatePage(page, "page");

        if (page.PageInfo != null)
        {
            PageInfo[] existingPageInfos = pageInfoOrchestrationService.GetAll(ignoreFilters: true)
                .Where(pageInfo => pageInfo.PageId == page.Id)
                .ToArray();

            await SyncPageInfoAsync(page.Id, existingPageInfos, page.PageInfo);
        }

        if (page.Contents != null)
        {
            Content[] existingContents = contentOrchestrationService.GetAll(ignoreFilters: true)
                .Where(content => content.PageId == page.Id)
                .ToArray();

            await SyncContentsAsync(page.Id, existingContents, page.Contents);
        }

        if (page.Roles != null)
        {
            PageRole[] existingPageRoles = pageRoleOrchestrationService.GetAll(ignoreFilters: true)
                .Where(pageRole => pageRole.PageId == page.Id)
                .ToArray();

            await SyncRolesAsync(page.Id, existingPageRoles, page.Roles);
        }

        int[] providedChildIds = [];

        if (page.Pages != null)
        {
            Page[] providedChildren = page.Pages
                .Select(child =>
                {
                    child.ParentId = page.Id;
                    child.AppId = page.AppId;
                    return child;
                })
                .ToArray();

            if (providedChildren.Length != 0)
                await pageOrchestrationService.AddOrUpdate(providedChildren);

            providedChildIds = providedChildren
                .Where(child => child.Id != 0)
                .Select(child => child.Id)
                .ToArray();
        }

        Page[] existingChildrenToRecompute = pageOrchestrationService.GetAll(ignoreFilters: true)
            .Where(child => child.ParentId == (int?)page.Id && !((ReadOnlySpan<int>)providedChildIds).Contains(child.Id))
            .ToArray();

        foreach (Page child in existingChildrenToRecompute)
        {
            child.ParentId = page.Id;
            child.AppId = page.AppId;
        }

        if (existingChildrenToRecompute.Length != 0)
            await pageOrchestrationService.AddOrUpdate(existingChildrenToRecompute);
    }

    public async ValueTask HandlePageDeleteAsync(Page page)
    {
        ValidatePage(page, "page");
        IEnumerable<PageRole> pageRolesToDelete = pageRoleOrchestrationService.GetAll(ignoreFilters: true)
            .Where(pageRole => pageRole.PageId == page.Id)
            .ToArray();

        IEnumerable<PageInfo> pageInfosToDelete = pageInfoOrchestrationService.GetAll(ignoreFilters: true)
            .Where(pageInfo => pageInfo.PageId == page.Id)
            .ToArray();

        IEnumerable<Content> contentsToDelete = contentOrchestrationService.GetAll(ignoreFilters: true)
            .Where(content => content.PageId == page.Id)
            .ToArray();

        await pageRoleOrchestrationService.DeleteAllAsync(pageRolesToDelete);
        await pageInfoOrchestrationService.DeleteAllAsync(pageInfosToDelete);
        await contentOrchestrationService.DeleteAllAsync(contentsToDelete);
    }

    private static Page ValidatePage(Page page, string parameterName)
    {
        if (page == null)
            throw new ValidationException(parameterName + " is required.");

        return page;
    }

    private async ValueTask SyncPageInfoAsync(
        int pageId,
        IEnumerable<PageInfo> existingItems,
        IEnumerable<PageInfo> incomingItems)
    {
        PageInfo[] existingArray = existingItems.ToArray();
        PageInfo[] incomingArray = incomingItems.ToArray();

        foreach (PageInfo incoming in incomingArray)
        {
            PageInfo existing = existingArray.FirstOrDefault(item =>
                string.Equals(item.CultureId, incoming.CultureId, StringComparison.Ordinal));

            if (existing == null)
            {
                await pageInfoOrchestrationService.AddAsync(new PageInfo
                {
                    PageId = pageId,
                    CultureId = incoming.CultureId,
                    Title = incoming.Title,
                    Description = incoming.Description,
                    Keywords = incoming.Keywords
                });

                continue;
            }

            await pageInfoOrchestrationService.UpdateAsync(new PageInfo
            {
                Id = existing.Id,
                PageId = pageId,
                CultureId = incoming.CultureId,
                Title = incoming.Title,
                Description = incoming.Description,
                Keywords = incoming.Keywords
            });
        }

        foreach (PageInfo existing in existingArray
            .Where(item => item.CultureId != string.Empty && !incomingArray.Any(incoming =>
                string.Equals(incoming.CultureId, item.CultureId, StringComparison.Ordinal))))
            await pageInfoOrchestrationService.DeleteAsync(existing.Id);
    }

    private async ValueTask SyncContentsAsync(
        int pageId,
        IEnumerable<Content> existingItems,
        IEnumerable<Content> incomingItems)
    {
        Content[] existingArray = existingItems.ToArray();
        Content[] incomingArray = incomingItems.ToArray();

        foreach (Content incoming in incomingArray)
        {
            Content existing = existingArray.FirstOrDefault(item =>
                string.Equals(item.Name, incoming.Name, StringComparison.Ordinal) &&
                string.Equals(item.CultureId, incoming.CultureId, StringComparison.Ordinal));

            if (existing == null)
            {
                await contentOrchestrationService.AddAsync(new Content
                {
                    PageId = pageId,
                    CultureId = incoming.CultureId,
                    Name = incoming.Name,
                    Html = incoming.Html
                });

                continue;
            }

            await contentOrchestrationService.UpdateAsync(new Content
            {
                Id = existing.Id,
                PageId = pageId,
                CultureId = incoming.CultureId,
                Name = incoming.Name,
                Html = incoming.Html
            });
        }

        foreach (Content existing in existingArray
            .Where(item => item.CultureId != string.Empty && !incomingArray.Any(incoming =>
                string.Equals(incoming.Name, item.Name, StringComparison.Ordinal) &&
                string.Equals(incoming.CultureId, item.CultureId, StringComparison.Ordinal))))
            await contentOrchestrationService.DeleteAsync(existing.Id);
    }

    private async ValueTask SyncRolesAsync(
        int pageId,
        IEnumerable<PageRole> existingItems,
        IEnumerable<PageRole> incomingItems)
    {
        PageRole[] existingArray = existingItems.ToArray();
        PageRole[] incomingArray = incomingItems.ToArray();

        foreach (PageRole incoming in incomingArray
            .Where(item => !existingArray.Any(existing => existing.RoleId == item.RoleId)))
        {
            await pageRoleOrchestrationService.AddAsync(new PageRole
            {
                PageId = pageId,
                RoleId = incoming.RoleId
            });
        }

        foreach (PageRole existing in existingArray
            .Where(item => !incomingArray.Any(incoming => incoming.RoleId == item.RoleId)))
        {
            await pageRoleOrchestrationService.DeleteAsync(new PageRole
            {
                PageId = pageId,
                RoleId = existing.RoleId
            });
        }
    }
}
