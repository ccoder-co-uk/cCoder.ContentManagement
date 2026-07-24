// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.ComponentModel.DataAnnotations;
using cCoder.ContentManagement.Services.Processings;
using cCoder.Data.Models.Packaging;

namespace cCoder.ContentManagement.Services.Orchestrations;

internal partial class MigrationSupportOrchestrationService(
    IJsonProcessingService jsonProcessingService,
    IPackageExportProcessingService packageExportProcessingService)
        : IMigrationSupportOrchestrationService
{
    public T[] DeserializeItems<T>(string json) =>
        TryCatch<T[]>(operation: () =>
    {
        ValidateDeserializeItems(inputs: [json]);
        return jsonProcessingService.DeserializeItems<T>(json: json);
    });

    public Package[] ExportPackages(int appId, string[] packageNames) =>
        TryCatch<Package[]>(operation: () =>
    {
        ValidateExportPackages(inputs: [appId, packageNames]);
        ValidateAppId(appId: appId, parameterName: "appId");

        return ValidatePackageNames(
            packageNames: packageNames,
            parameterName: "packageNames")
            .Select(selector: packageName =>
                packageExportProcessingService.ExportPackage(
                    appId: appId,
                    packageName: packageName))
            .ToArray();

    });

    private static void ValidateAppId(int appId, string parameterName) =>
        ThrowIf(
            condition: appId < 1,
            message: parameterName + " must be greater than 0.");

    private static string[] ValidatePackageNames(
        string[] packageNames,
        string parameterName)
    {
        if (packageNames == null || packageNames.Length == 0)
        {
            throw new ValidationException(
                message: parameterName + " is required.");
        }

        return packageNames;
    }

    private static void ThrowIf(bool condition, string message)
    {
        if (condition)
        {
            throw new ValidationException(message: message);
        }
    }
}