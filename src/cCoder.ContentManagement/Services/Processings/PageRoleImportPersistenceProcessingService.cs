// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.ComponentModel.DataAnnotations;
using cCoder.ContentManagement.Services.Foundations.Storages;
using cCoder.Data.Models.CMS;
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

        int[] pageIds = pageRoles
            .Select(selector: pageRole => pageRole.PageId)
            .Distinct()
            .ToArray();

        PageRole[] existingPageRoles = service
            .GetAllPageRolesIgnoringFilters()
            .Where(
                predicate: pageRole =>
                    Enumerable.Contains(
                        source: pageIds,
                        value: pageRole.PageId))
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
            await service.DeleteAllPageRolesAsync(
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
}