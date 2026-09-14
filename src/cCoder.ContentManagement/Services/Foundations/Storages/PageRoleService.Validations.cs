// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Security;
using System.ComponentModel.DataAnnotations;

namespace cCoder.ContentManagement.Services.Foundations.Storages;

internal partial class PageRoleService
{
    private static void ValidatePageRoleForImportOnAdd(object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateAllPageRolesIgnoringFiltersOnGet(object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidatePageRoleAuthorization(object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidatePageRoleExistence(object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidatePageRoleOnResolve(object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateAllPageRolesOnDelete(object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidatePageId(int pageId, string parameterName) =>
        ThrowIf(condition: pageId < 1, message: parameterName + " must be greater than 0.");

    private static void ValidateRoleId(Guid roleId, string parameterName) =>
        ThrowIf(condition: roleId == Guid.Empty, message: parameterName + " is required.");

    private static void ValidatePageRole(PageRole pageRole, string parameterName)
    {
        if (pageRole == null)
        {
            throw new ValidationException(message: parameterName + " is required.");
        }

        if (pageRole.PageId < 1)
        {
            throw new ValidationException(message: parameterName + ".PageId must be greater than 0.");
        }

        if (pageRole.RoleId == Guid.Empty)
        {
            throw new ValidationException(message: parameterName + ".RoleId is required.");
        }
    }

    private static void ThrowIf(bool condition, string message)
    {
        if (condition)
        {
            throw new ValidationException(message: message);
        }
    }

    private static void ValidateAllPageRoleOnGet(object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidatePageRoleOnAdd(object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidatePageRoleOnDelete(object[] inputs) =>
        Validate(inputs: inputs);

    private static void Validate(params object[] inputs) =>
        _ = inputs;
}