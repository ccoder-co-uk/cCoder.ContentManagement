// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Packaging;

namespace cCoder.ContentManagement.Services.Processings;

internal interface IPackageExportProcessingService
{
    Package ExportPackage(int appId, string packageName);
}