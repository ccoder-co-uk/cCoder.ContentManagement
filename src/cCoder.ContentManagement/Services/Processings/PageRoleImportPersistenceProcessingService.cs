// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Collections.Generic;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using cCoder.ContentManagement.Services.Foundations.Storages;
using cCoder.Data.Models.Security;

namespace cCoder.ContentManagement.Services.Processings;

internal interface IPageRoleImportPersistenceProcessingService
{
    ValueTask SynchronizePageRolesAsync(PageRole[] pageRoles);
}

internal sealed partial class PageRoleImportPersistenceProcessingService(
    IPageRoleService service)
        : IPageRoleImportPersistenceProcessingService
{
    public ValueTask SynchronizePageRolesAsync(PageRole[] pageRoles) =>
        TryCatch(operation: async () =>
    {
        ValidatePageRolesOnSynchronize(inputs: [pageRoles]);
        ValidatePageRoles(pageRoles: pageRoles, parameterName: "pageRoles");

        List<int> pageIds = [];

        foreach (PageRole pageRole in pageRoles)
        {
            if (!ContainsPageId(pageIds: pageIds, pageId: pageRole.PageId))
            {
                pageIds.Add(item: pageRole.PageId);
            }
        }

        List<PageRole> existingPageRoles = [];

        foreach (PageRole existing in service.GetAllPageRolesIgnoringFilters())
        {
            if (ContainsPageId(pageIds: pageIds, pageId: existing.PageId))
            {
                existingPageRoles.Add(item: existing);
            }
        }

        List<PageRole> pageRolesToDelete = [];

        foreach (PageRole existing in existingPageRoles)
        {
            if (!ContainsPageRole(pageRoles: pageRoles, candidate: existing))
            {
                pageRolesToDelete.Add(item: existing);
            }
        }

        if (pageRolesToDelete.Count > 0)
        {
            PageRole[] deletedPageRoles = [.. pageRolesToDelete];

            await service.DeleteAllPageRolesAsync(
                deletedPageRole: deletedPageRoles);
        }

        foreach (PageRole pageRole in pageRoles)
        {
            if (ContainsPageRole(
                pageRoles: existingPageRoles,
                candidate: pageRole))
            {
                continue;
            }

            await service.AddPageRoleForImportAsync(
                newPageRole: pageRole);
        }
    }, isValueTask: true);

    private static void ValidatePageRoles(
        IEnumerable<PageRole> pageRoles,
        string parameterName)
    {
        if (pageRoles == null)
        {
            throw new ValidationException(
                message: parameterName + " is required.");
        }
    }

    private static bool ContainsPageId(
        IEnumerable<int> pageIds,
        int pageId)
    {
        foreach (int existingPageId in pageIds)
        {
            if (existingPageId == pageId)
            {
                return true;
            }
        }

        return false;
    }

    private static bool ContainsPageRole(
        IEnumerable<PageRole> pageRoles,
        PageRole candidate)
    {
        foreach (PageRole pageRole in pageRoles)
        {
            if (pageRole.PageId == candidate.PageId
                && pageRole.RoleId == candidate.RoleId)
            {
                return true;
            }
        }

        return false;
    }
}