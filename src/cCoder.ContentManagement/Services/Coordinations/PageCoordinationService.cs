using System.ComponentModel.DataAnnotations;
using cCoder.ContentManagement.Services.Orchestrations;
using Content = cCoder.Data.Models.CMS.Content;
using Page = cCoder.Data.Models.CMS.Page;
using PageInfo = cCoder.Data.Models.CMS.PageInfo;
using PageRole = cCoder.Data.Models.Security.PageRole;

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
        PageInfo[] pageInfos = (page.PageInfo ?? new List<PageInfo>()).Select((PageInfo pageInfo) => new PageInfo
        {
            Id = pageInfo.Id,
            PageId = page.Id,
            CultureId = pageInfo.CultureId,
            Title = pageInfo.Title,
            Description = pageInfo.Description,
            Keywords = pageInfo.Keywords
        }).ToArray();
        Content[] contents = (page.Contents ?? new List<Content>()).Select(delegate (Content content)
        {
            content.PageId = page.Id;
            return content;
        }).ToArray();
        PageRole[] pageRoles = (page.Roles ?? new List<PageRole>()).Select(pageRole => new PageRole
        {
            PageId = page.Id,
            RoleId = pageRole.RoleId
        }).ToArray();
        await pageInfoOrchestrationService.AddOrUpdate(pageInfos);
        await contentOrchestrationService.AddOrUpdate(contents);
        await pageRoleOrchestrationService.AddOrUpdate(pageRoles);
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

        Page[] providedChildren = (page.Pages ?? new List<Page>()).Select(delegate (Page child)
        {
            child.ParentId = page.Id;
            child.AppId = page.AppId;
            return child;
        }).ToArray();
        if (providedChildren.Length != 0)
        {
            await pageOrchestrationService.AddOrUpdate(providedChildren);
        }
        int[] providedChildIds = (from child in providedChildren
                                  where child.Id != 0
                                  select child.Id).ToArray();
        Page[] existingChildrenToRecompute = (from child in pageOrchestrationService.GetAll(ignoreFilters: true)
                                              where child.ParentId == (int?)page.Id && !((ReadOnlySpan<int>)providedChildIds).Contains(child.Id)
                                              select child).ToArray();
        existingChildrenToRecompute.ToList().ForEach(delegate (Page child)
        {
            child.ParentId = page.Id;
            child.AppId = page.AppId;
        });
        if (existingChildrenToRecompute.Length != 0)
        {
            await pageOrchestrationService.AddOrUpdate(existingChildrenToRecompute);
        }
    }

    public async ValueTask HandlePageDeleteAsync(Page page)
    {
        ValidatePage(page, "page");
        IEnumerable<PageRole> pageRolesToDelete = (from pageRole in pageRoleOrchestrationService.GetAll(ignoreFilters: true)
                                                   where pageRole.PageId == page.Id
                                                   select pageRole).ToArray();
        IEnumerable<PageInfo> pageInfosToDelete = (from pageInfo in pageInfoOrchestrationService.GetAll(ignoreFilters: true)
                                                   where pageInfo.PageId == page.Id
                                                   select pageInfo).ToArray();
        IEnumerable<Content> contentsToDelete = (from content in contentOrchestrationService.GetAll(ignoreFilters: true)
                                                 where content.PageId == page.Id
                                                 select content).ToArray();
        await pageRoleOrchestrationService.DeleteAllAsync(pageRolesToDelete);
        await pageInfoOrchestrationService.DeleteAllAsync(pageInfosToDelete);
        await contentOrchestrationService.DeleteAllAsync(contentsToDelete);
    }

    private static Page ValidatePage(Page page, string parameterName)
    {
        if (page == null)
        {
            throw new ValidationException(parameterName + " is required.");
        }
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
        {
            await pageInfoOrchestrationService.DeleteAsync(existing.Id);
        }
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
        {
            await contentOrchestrationService.DeleteAsync(existing.Id);
        }
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
