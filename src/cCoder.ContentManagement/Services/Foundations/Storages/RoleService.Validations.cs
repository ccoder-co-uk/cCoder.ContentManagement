// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

namespace cCoder.ContentManagement.Services.Foundations.Storages;

internal sealed partial class RoleService
{
    private static void ValidateAllRolesIgnoringFiltersOnGet(object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateRoleOnAdd(object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateRoleOnUpdate(object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateRole(cCoder.Data.Models.Security.Role role, string parameterName)
    {
        if (role == null)
        {
            throw new System.ComponentModel.DataAnnotations.ValidationException(message: parameterName + " is required.");
        }
    }

    private static void Validate(params object[] inputs) =>
        _ = inputs;
}