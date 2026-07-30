// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Packaging;

namespace cCoder.ContentManagement.Services.Foundations.Exports;

internal interface IPackageExportService
{
    Package ExportRolesPackage(int appId);

    Package ExportLayoutsPackage(int appId);

    Package ExportTemplatesPackage(int appId);

    Package ExportComponentsPackage(int appId);

    Package ExportScriptsPackage(int appId);

    Package ExportResourcesPackage(int appId);

    Package ExportPagesPackage(int appId);

    Package ExportPageRolesPackage(int appId);
}