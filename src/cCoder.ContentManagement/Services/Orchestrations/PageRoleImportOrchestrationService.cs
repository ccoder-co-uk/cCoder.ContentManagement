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

        PageRole[] pageRoles = pageRoleInfos
            .Select(
                selector: pageRoleInfo =>
                    lookupProcessingService.ResolvePageRole(
                        appId: appId,
                        path: pageRoleInfo.Path,
                        roleName: pageRoleInfo.Role))
            .Where(
                predicate: pageRole =>
                    pageRole.PageId != 0
                    && pageRole.RoleId != Guid.Empty)
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