// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Packaging;

namespace cCoder.ContentManagement.Exposures;

public interface IContentManagementPackageManager
{
    ValueTask ImportPackageAsync(int? appId, Package package);

    Package ExportPackage(int appId, string packageName);
}