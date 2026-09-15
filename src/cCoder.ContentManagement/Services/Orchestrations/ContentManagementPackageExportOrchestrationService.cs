// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Security;
using cCoder.ContentManagement.Models;
using cCoder.ContentManagement.Services.Processings;
using cCoder.Data.Models.Packaging;

namespace cCoder.ContentManagement.Services.Orchestrations;

internal sealed partial class ContentManagementPackageExportOrchestrationService(
    IPackageExportProcessingService packageExportProcessingService,
    IAuthorizationProcessingService authorizationProcessingService)
    : IContentManagementPackageExportOrchestrationService
{
    public Package ExportPackage(int appId, string packageName) =>
        TryCatch(operation: () =>
    {
        ValidateExportPackage(inputs: [appId, packageName]);

        bool isAdmin = authorizationProcessingService
            .IsAdminOfAppAuthorizationContext(
                context: new AuthorizationContext { AppId = appId });

        if (!isAdmin)
        {
            throw new SecurityException(message: "Access Denied!");
        }

        return packageExportProcessingService.ExportPackage(
            appId: appId,
            packageName: packageName);
    });
}