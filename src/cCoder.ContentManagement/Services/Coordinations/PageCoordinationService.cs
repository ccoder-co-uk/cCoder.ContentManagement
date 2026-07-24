// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

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
        ValidatePage(page: page, parameterName: "page");

        if (page.PageInfo != null)
        {
            PageInfo[] pageInfos = page.PageInfo.Select(selector: (PageInfo pageInfo) => new PageInfo
            {
                Id = pageInfo.Id,
                PageId = page.Id,
                CultureId = pageInfo.CultureId,
                Title = pageInfo.Title,
                Description = pageInfo.Description,
                Keywords = pageInfo.Keywords
            })
                .ToArray();

            await pageInfoOrchestrationService.AddOrUpdatePageInfoResult(newPageInfo: pageInfos);
        }

        if (page.Contents != null)
        {
            Content[] contents = page.Contents
                .Select(selector: content =>
                {
                    content.PageId = page.Id;
                    return content;
                })
                .ToArray();

            await contentOrchestrationService.AddOrUpdateContentResult(newContent: contents);
        }

        if (page.Roles != null)
        {
            PageRole[] pageRoles = page.Roles.Select(selector: pageRole => new PageRole
            {
                PageId = page.Id,
                RoleId = pageRole.RoleId
            })
                .ToArray();

            await pageRoleOrchestrationService.AddOrUpdatePageRoleResult(newPageRole: pageRoles);
        }
    }

    public async ValueTask HandlePageUpdateAsync(Page page)
    {
        ValidatePage(page: page, parameterName: "page");

        if (page.PageInfo != null)
        {
            PageInfo[] existingPageInfos = pageInfoOrchestrationService.GetAllPageInfo(ignoreFilters: true)
                .Where(predicate: pageInfo => pageInfo.PageId == page.Id)
                .ToArray();

            await SyncPageInfoAsync(pageId: page.Id, existingItems: existingPageInfos, incomingItems: page.PageInfo);
        }

        if (page.Contents != null)
        {
            Content[] existingContents = contentOrchestrationService.GetAllContent(ignoreFilters: true)
                .Where(predicate: content => content.PageId == page.Id)
                .ToArray();

            await SyncContentsAsync(pageId: page.Id, existingItems: existingContents, incomingItems: page.Contents);
        }

        if (page.Roles != null)
        {
            PageRole[] existingPageRoles = pageRoleOrchestrationService.GetAllPageRole(ignoreFilters: true)
                .Where(predicate: pageRole => pageRole.PageId == page.Id)
                .ToArray();

            await SyncRolesAsync(pageId: page.Id, existingItems: existingPageRoles, incomingItems: page.Roles);
        }

        int[] providedChildIds = [];

        if (page.Pages != null)
        {
            Page[] providedChildren = page.Pages
                .Select(selector: child =>
                {
                    child.ParentId = page.Id;
                    child.AppId = page.AppId;
                    return child;
                })
                .ToArray();

            if (providedChildren.Length != 0)
            {
                await pageOrchestrationService.AddOrUpdatePageResult(newPage: providedChildren);
            }

            providedChildIds = providedChildren
                .Where(predicate: child => child.Id != 0)
                .Select(selector: child => child.Id)
                .ToArray();
        }

        Page[] existingChildrenToRecompute = pageOrchestrationService.GetAllPage(ignoreFilters: true)
            .Where(predicate: child => child.ParentId == (int?)page.Id && !((ReadOnlySpan<int>)providedChildIds).Contains(value: child.Id))
            .ToArray();

        foreach (Page child in existingChildrenToRecompute)
        {
            child.ParentId = page.Id;
            child.AppId = page.AppId;
        }

        if (existingChildrenToRecompute.Length != 0)
        {
            await pageOrchestrationService.AddOrUpdatePageResult(newPage: existingChildrenToRecompute);
        }
    }

    public async ValueTask HandlePageDeleteAsync(Page page)
    {
        ValidatePage(page: page, parameterName: "page");

        IEnumerable<PageRole> pageRolesToDelete = pageRoleOrchestrationService.GetAllPageRole(ignoreFilters: true)
            .Where(predicate: pageRole => pageRole.PageId == page.Id)
            .ToArray();

        IEnumerable<PageInfo> pageInfosToDelete = pageInfoOrchestrationService.GetAllPageInfo(ignoreFilters: true)
            .Where(predicate: pageInfo => pageInfo.PageId == page.Id)
            .ToArray();

        IEnumerable<Content> contentsToDelete = contentOrchestrationService.GetAllContent(ignoreFilters: true)
            .Where(predicate: content => content.PageId == page.Id)
            .ToArray();

        await pageRoleOrchestrationService.DeleteAllPageRoleAsync(deletedPageRole: pageRolesToDelete);
        await pageInfoOrchestrationService.DeleteAllPageInfoAsync(deletedPageInfo: pageInfosToDelete);
        await contentOrchestrationService.DeleteAllContentAsync(deletedContent: contentsToDelete);
    }

    private static Page ValidatePage(Page page, string parameterName)
    {
        if (page == null)
        {
            throw new ValidationException(message: parameterName + " is required.");
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
            PageInfo existing = existingArray.FirstOrDefault(predicate: item =>
                string.Equals(a: item.CultureId, b: incoming.CultureId, comparisonType: StringComparison.Ordinal));

            if (existing == null)
            {
                await pageInfoOrchestrationService.AddPageInfoAsync(newPageInfo: new PageInfo
                {
                    PageId = pageId,
                    CultureId = incoming.CultureId,
                    Title = incoming.Title,
                    Description = incoming.Description,
                    Keywords = incoming.Keywords
                });

                continue;
            }

            await pageInfoOrchestrationService.UpdatePageInfoAsync(updatedPageInfo: new PageInfo
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
            .Where(predicate: item => item.CultureId != string.Empty && !incomingArray.Any(predicate: incoming =>
                string.Equals(a: incoming.CultureId, b: item.CultureId, comparisonType: StringComparison.Ordinal))))
        {
            await pageInfoOrchestrationService.DeleteAsync(pageInfoId: existing.Id);
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
            Content existing = existingArray.FirstOrDefault(predicate: item =>
                string.Equals(a: item.Name, b: incoming.Name, comparisonType: StringComparison.Ordinal) &&
                string.Equals(a: item.CultureId, b: incoming.CultureId, comparisonType: StringComparison.Ordinal));

            if (existing == null)
            {
                await contentOrchestrationService.AddContentAsync(newContent: new Content
                {
                    PageId = pageId,
                    CultureId = incoming.CultureId,
                    Name = incoming.Name,
                    Html = incoming.Html
                });

                continue;
            }

            await contentOrchestrationService.UpdateContentAsync(updatedContent: new Content
            {
                Id = existing.Id,
                PageId = pageId,
                CultureId = incoming.CultureId,
                Name = incoming.Name,
                Html = incoming.Html
            });
        }

        foreach (Content existing in existingArray
            .Where(predicate: item => item.CultureId != string.Empty && !incomingArray.Any(predicate: incoming =>
                string.Equals(a: incoming.Name, b: item.Name, comparisonType: StringComparison.Ordinal) &&
                string.Equals(a: incoming.CultureId, b: item.CultureId, comparisonType: StringComparison.Ordinal))))
        {
            await contentOrchestrationService.DeleteAsync(contentId: existing.Id);
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
            .Where(predicate: item => !existingArray.Any(predicate: existing => existing.RoleId == item.RoleId)))
        {
            await pageRoleOrchestrationService.AddPageRoleAsync(newPageRole: new PageRole
            {
                PageId = pageId,
                RoleId = incoming.RoleId
            });
        }

        foreach (PageRole existing in existingArray
            .Where(predicate: item => !incomingArray.Any(predicate: incoming => incoming.RoleId == item.RoleId)))
        {
            await pageRoleOrchestrationService.DeletePageRoleAsync(deletedPageRole: new PageRole
            {
                PageId = pageId,
                RoleId = existing.RoleId
            });
        }
    }
}