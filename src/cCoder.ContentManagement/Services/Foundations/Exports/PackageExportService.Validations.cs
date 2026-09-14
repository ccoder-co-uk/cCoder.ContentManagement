// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.ComponentModel.DataAnnotations;

namespace cCoder.ContentManagement.Services.Foundations.Exports;

internal partial class PackageExportService
{
    private static int ValidateAppId(int appId, string parameterName)
    {
        if (appId < 1)
        {
            throw new ValidationException(message: parameterName + " must be greater than 0.");
        }

        return appId;
    }

    private static void ValidateExportRolesPackage(object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateExportLayoutsPackage(object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateExportTemplatesPackage(object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateExportComponentsPackage(object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateExportScriptsPackage(object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateExportResourcesPackage(object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateExportPagesPackage(object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateExportPageRolesPackage(object[] inputs) =>
        Validate(inputs: inputs);

    private static void Validate(params object[] inputs) =>
        _ = inputs;
}