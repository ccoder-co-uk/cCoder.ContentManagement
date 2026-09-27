// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Packaging;

namespace cCoder.ContentManagement.Services.Coordinations;

internal interface IContentManagementPackageCoordinationService
{
    ValueTask ImportPackageAsync(int? appId, Package package);

    Package ExportPackage(int appId, string packageName);
}