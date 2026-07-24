// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.ComponentModel.DataAnnotations;
using cCoder.ContentManagement.Brokers;
using cCoder.ContentManagement.Brokers.Storages;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Security;

namespace cCoder.ContentManagement.Services.Processings;

internal interface IPageRoleImportLookupProcessingService
{
    PageRole ResolvePageRole(int appId, string path, string roleName);
}

internal sealed partial class PageRoleImportLookupProcessingService(
    IRoleBroker roleBroker,
    IPageBroker pageBroker)
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
        ValidateText(value: path, parameterName: "path");
        ValidateText(value: roleName, parameterName: "roleName");

        Role role = roleBroker.GetAllRoles(ignoreFilters: true)
            .FirstOrDefault(
                predicate: existing =>
                    existing.AppId == appId
                    && existing.Name == roleName);

        Page page = pageBroker.GetAllPages(ignoreFilters: true)
            .FirstOrDefault(
                predicate: existing =>
                    existing.AppId == appId
                    && existing.Path == path);

        return new PageRole
        {
            PageId = page?.Id ?? 0,
            RoleId = role?.Id ?? Guid.Empty
        };
    });

    private static void ValidateAppId(int appId, string parameterName) =>
        ThrowIf(
            condition: appId < 1,
            message: parameterName + " must be greater than 0.");

    private static void ValidateText(string value, string parameterName) =>
        ThrowIf(
            condition: string.IsNullOrWhiteSpace(value: value),
            message: parameterName + " is required.");

    private static void ThrowIf(bool condition, string message)
    {
        if (condition)
        {
            throw new ValidationException(message: message);
        }
    }
}