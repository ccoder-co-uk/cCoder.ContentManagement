// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

namespace cCoder.ContentManagement.Services.Foundations.Storages;

internal sealed partial class UserRoleService
{
    private static void ValidateAllUserRolesIgnoringFiltersOnGet(object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateUserRoleOnAdd(object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateAllUserRolesOnDelete(object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateUserRole(cCoder.Data.Models.Security.UserRole userRole, string parameterName)
    {
        if (userRole == null)
        {
            throw new System.ComponentModel.DataAnnotations.ValidationException(message: parameterName + " is required.");
        }
    }

    private static void Validate(params object[] inputs) =>
        _ = inputs;
}