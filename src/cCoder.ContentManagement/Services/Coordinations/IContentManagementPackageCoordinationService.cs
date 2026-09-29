// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Threading.Tasks;
using cCoder.Data.Models.Packaging;

namespace cCoder.ContentManagement.Services.Coordinations;

internal interface IContentManagementPackageCoordinationService
{
    ValueTask ImportPackageAsync(int? appId, Package package);

    Package ExportPackage(int appId, string packageName);
}