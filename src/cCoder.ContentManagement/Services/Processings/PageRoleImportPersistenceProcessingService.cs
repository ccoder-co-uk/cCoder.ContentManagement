// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.ComponentModel.DataAnnotations;
using cCoder.ContentManagement.Brokers.Storages;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Security;

namespace cCoder.ContentManagement.Services.Processings;

internal interface IPageRoleImportPersistenceProcessingService
{
    ValueTask SynchronizePageRolesAsync(PageRole[] pageRoles);
}

internal sealed partial class PageRoleImportPersistenceProcessingService(
    IPageRoleBroker pageRoleBroker)
        : IPageRoleImportPersistenceProcessingService
{
    public ValueTask SynchronizePageRolesAsync(PageRole[] pageRoles) =>
        TryCatch(operation: async () =>
    {
        ValidatePageRolesOnSynchronize(inputs: [pageRoles]);
        ValidatePageRoles(pageRoles: pageRoles, parameterName: "pageRoles");

        int[] pageIds = pageRoles
            .Select(selector: pageRole => pageRole.PageId)
            .Distinct()
            .ToArray();

        PageRole[] existingPageRoles = pageRoleBroker
            .GetAllPageRoles(ignoreFilters: true)
            .Where(
                predicate: pageRole =>
                    ((ReadOnlySpan<int>)pageIds)
                        .Contains(value: pageRole.PageId))
            .ToArray();

        PageRole[] pageRolesToDelete = existingPageRoles
            .Where(
                predicate: existing =>
                    !pageRoles.Any(
                        predicate: incoming =>
                            incoming.PageId == existing.PageId
                            && incoming.RoleId == existing.RoleId))
            .ToArray();

        if (pageRolesToDelete.Length > 0)
        {
            await pageRoleBroker.DeleteAllPageRolesAsync(
                deletedPageRole: pageRolesToDelete);
        }

        foreach (
            PageRole pageRole in pageRoles.Where(
                predicate: incoming =>
                    !existingPageRoles.Any(
                        predicate: existing =>
                            existing.PageId == incoming.PageId
                            && existing.RoleId == incoming.RoleId)))
        {
            await pageRoleBroker.AddPageRoleAsync(
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
}