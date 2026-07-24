// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.ComponentModel.DataAnnotations;
using cCoder.ContentManagement.Services.Orchestrations;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Security;

namespace cCoder.ContentManagement.Services.Coordinations;

internal partial class PageStructureCoordinationService(
    IPageRoleOrchestrationService pageRoleOrchestrationService,
    IPageOrchestrationService pageOrchestrationService)
        : IPageStructureCoordinationService
{
    public ValueTask HandlePageAddAsync(Page page) =>
        TryCatch(operation: async () =>
    {
        ValidateHandlePageAddAsync(inputs: [page]);
        ValidatePage(page: page, parameterName: "page");

        if (page.Roles != null)
        {
            PageRole[] pageRoles = page.Roles
                .Select(selector: pageRole => new PageRole
                {
                    PageId = page.Id,
                    RoleId = pageRole.RoleId
                })
                .ToArray();

            await pageRoleOrchestrationService.AddOrUpdatePageRoleResult(
                newPageRole: pageRoles);
        }

    }, isValueTask: true);

    public ValueTask HandlePageUpdateAsync(Page page) =>
        TryCatch(operation: async () =>
    {
        ValidateHandlePageUpdateAsync(inputs: [page]);
        ValidatePage(page: page, parameterName: "page");

        if (page.Roles != null)
        {
            PageRole[] existingPageRoles = pageRoleOrchestrationService
                .GetAllPageRole(ignoreFilters: true)
                .Where(predicate: pageRole => pageRole.PageId == page.Id)
                .ToArray();

            await SyncRolesAsync(
                pageId: page.Id,
                existingItems: existingPageRoles,
                incomingItems: page.Roles);
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
                await pageOrchestrationService.AddOrUpdatePageResult(
                    newPage: providedChildren);
            }

            providedChildIds = providedChildren
                .Where(predicate: child => child.Id != 0)
                .Select(selector: child => child.Id)
                .ToArray();
        }

        Page[] existingChildrenToRecompute = pageOrchestrationService
            .GetAllPage(ignoreFilters: true)
            .Where(predicate: child =>
                child.ParentId == (int?)page.Id &&
                !((ReadOnlySpan<int>)providedChildIds).Contains(value: child.Id))
            .ToArray();

        foreach (Page child in existingChildrenToRecompute)
        {
            child.ParentId = page.Id;
            child.AppId = page.AppId;
        }

        if (existingChildrenToRecompute.Length != 0)
        {
            await pageOrchestrationService.AddOrUpdatePageResult(
                newPage: existingChildrenToRecompute);
        }

    }, isValueTask: true);

    public ValueTask HandlePageDeleteAsync(Page page) =>
        TryCatch(operation: async () =>
    {
        ValidateHandlePageDeleteAsync(inputs: [page]);
        ValidatePage(page: page, parameterName: "page");

        PageRole[] pageRolesToDelete = pageRoleOrchestrationService
            .GetAllPageRole(ignoreFilters: true)
            .Where(predicate: pageRole => pageRole.PageId == page.Id)
            .ToArray();

        await pageRoleOrchestrationService.DeleteAllPageRoleAsync(
            deletedPageRole: pageRolesToDelete);

    }, isValueTask: true);

    private async ValueTask SyncRolesAsync(
        int pageId,
        IEnumerable<PageRole> existingItems,
        IEnumerable<PageRole> incomingItems)
    {
        PageRole[] existingArray = existingItems.ToArray();
        PageRole[] incomingArray = incomingItems.ToArray();

        foreach (PageRole incoming in incomingArray
            .Where(predicate: item =>
                !existingArray.Any(predicate: existing => existing.RoleId == item.RoleId)))
        {
            PageRole newPageRole = new()
            {
                PageId = pageId,
                RoleId = incoming.RoleId
            };

            await pageRoleOrchestrationService.AddPageRoleAsync(
                newPageRole: newPageRole);
        }

        foreach (PageRole existing in existingArray
            .Where(predicate: item =>
                !incomingArray.Any(predicate: incoming => incoming.RoleId == item.RoleId)))
        {
            PageRole deletedPageRole = new()
            {
                PageId = pageId,
                RoleId = existing.RoleId
            };

            await pageRoleOrchestrationService.DeletePageRoleAsync(
                deletedPageRole: deletedPageRole);
        }
    }

    private static Page ValidatePage(Page page, string parameterName)
    {
        if (page == null)
        {
            throw new ValidationException(message: parameterName + " is required.");
        }

        return page;
    }
}