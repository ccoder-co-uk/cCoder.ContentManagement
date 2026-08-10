// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.ComponentModel.DataAnnotations;
using cCoder.ContentManagement.Models;
using cCoder.ContentManagement.Services.Processings;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Security;

namespace cCoder.ContentManagement.Services.Orchestrations;

internal interface IPageRoleImportOrchestrationService
{
    ValueTask ImportPageRoleInfosAsync(
        int appId,
        PageRoleInfo[] pageRoleInfos);
}

internal sealed partial class PageRoleImportOrchestrationService(
    IPageRoleImportLookupProcessingService lookupProcessingService,
    IPageRoleImportPersistenceProcessingService persistenceProcessingService)
        : IPageRoleImportOrchestrationService
{
    private const int LookupRetryCount = 50;
    private const int LookupRetryDelayMilliseconds = 100;

    public ValueTask ImportPageRoleInfosAsync(
        int appId,
        PageRoleInfo[] pageRoleInfos) =>
        TryCatch(operation: async () =>
    {
        ValidatePageRoleInfosOnImport(inputs: [appId, pageRoleInfos]);
        ValidateAppId(appId: appId, parameterName: "appId");

        ValidatePageRoleInfos(
            pageRoleInfos: pageRoleInfos,
            parameterName: "pageRoleInfos");

        PageRole[] pageRoles = await ResolvePageRolesAsync(
            appId: appId,
            pageRoleInfos: pageRoleInfos);

        pageRoles = pageRoles
            .GroupBy(
                keySelector: pageRole =>
                    new
                    {
                        pageRole.PageId,
                        pageRole.RoleId
                    })
            .Select(selector: group => group.First())
            .ToArray();

        await persistenceProcessingService.SynchronizePageRolesAsync(
            pageRoles: pageRoles);
    }, isValueTask: true);

    private async ValueTask<PageRole[]> ResolvePageRolesAsync(
        int appId,
        PageRoleInfo[] pageRoleInfos)
    {
        PageRole[] pageRoles = [];

        for (int attempt = 0; attempt < LookupRetryCount; attempt++)
        {
            pageRoles = pageRoleInfos
                .Select(
                    selector: pageRoleInfo =>
                        lookupProcessingService.ResolvePageRole(
                            appId: appId,
                            path: pageRoleInfo.Path,
                            roleName: pageRoleInfo.Role))
                .ToArray();

            if (pageRoles.All(predicate: IsResolved))
            {
                return pageRoles;
            }

            await Task.Delay(millisecondsDelay: LookupRetryDelayMilliseconds);
        }

        throw new ValidationException(
            message: "Page roles could not be resolved after their pages and roles were imported.");
    }

    private static bool IsResolved(PageRole pageRole) =>
        pageRole.PageId != 0
        && pageRole.RoleId != Guid.Empty;

    private static void ValidateAppId(int appId, string parameterName) =>
        ThrowIf(
            condition: appId < 1,
            message: parameterName + " must be greater than 0.");

    private static void ValidatePageRoleInfos(
        IEnumerable<PageRoleInfo> pageRoleInfos,
        string parameterName)
    {
        if (pageRoleInfos == null)
        {
            throw new ValidationException(
                message: parameterName + " is required.");
        }
    }

    private static void ThrowIf(bool condition, string message)
    {
        if (condition)
        {
            throw new ValidationException(message: message);
        }
    }
}