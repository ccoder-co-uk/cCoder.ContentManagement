// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Services.Orchestrations;
using cCoder.Data.Models.Packaging;
using System.ComponentModel.DataAnnotations;

namespace cCoder.ContentManagement.Services.Aggregations;

internal partial class ContentManagementMigrationAggregationService(
    IMigrationSupportOrchestrationService migrationSupportOrchestrationService)
        : IContentManagementMigrationAggregationService
{
    public Package[] ExportPackages(int appId, string[] packageNames) =>
        TryCatch<Package[]>(operation: () =>
    {
        ValidateExportPackages(inputs: [appId, packageNames]);

        ValidateAppId(appId: appId, parameterName: "appId");

        return migrationSupportOrchestrationService.ExportPackages(
            appId: appId,
            packageNames: ValidatePackageNames(
                packageNames: packageNames,
                parameterName: "packageNames"));

    });

    private static int ValidateAppId(int appId, string parameterName)
    {
        if (appId < 1)
        {
            throw new ValidationException(
                message: parameterName + " must be greater than 0.");
        }

        return appId;
    }

    private static string[] ValidatePackageNames(
        string[] packageNames,
        string parameterName) =>
        packageNames
        ?? throw new ValidationException(
            message: parameterName + " is required.");
}