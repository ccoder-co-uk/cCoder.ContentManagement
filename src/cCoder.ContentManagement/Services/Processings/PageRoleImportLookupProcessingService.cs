// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.ComponentModel.DataAnnotations;
using cCoder.ContentManagement.Services.Foundations.Storages;
using cCoder.Data.Models;
using cCoder.Data.Models.Security;

namespace cCoder.ContentManagement.Services.Processings;

internal interface IPageRoleImportLookupProcessingService
{
    PageRole ResolvePageRole(int appId, string path, string roleName);
}

internal sealed partial class PageRoleImportLookupProcessingService(
    IPageRoleService service)
        : IPageRoleImportLookupProcessingService
{
    public PageRole ResolvePageRole(
        int appId,
        string path,
        string roleName) =>
        TryCatch<PageRole>(operation: () =>
    {
        ValidatePageRoleOnResolve(inputs: [appId, path, roleName]);
        ValidateAppId(appId: appId, parameterName: "appId");
        ValidatePath(value: path, parameterName: "path");
        ValidateText(value: roleName, parameterName: "roleName");

        return service.ResolvePageRole(
            appId: appId,
            path: path,
            roleName: roleName);
    });

    private static void ValidateAppId(int appId, string parameterName) =>
        ThrowIf(
            condition: appId < 1,
            message: parameterName + " must be greater than 0.");

    private static void ValidateText(string value, string parameterName) =>
        ThrowIf(
            condition: string.IsNullOrWhiteSpace(value: value),
            message: parameterName + " is required.");

    private static void ValidatePath(string value, string parameterName) =>
        ThrowIf(
            condition: value is null
                || (value.Length > 0 && string.IsNullOrWhiteSpace(value: value)),
            message: parameterName + " is required.");

    private static void ThrowIf(bool condition, string message)
    {
        if (condition)
        {
            throw new ValidationException(message: message);
        }
    }
}