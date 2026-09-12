// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Models;

namespace cCoder.ContentManagement.Services.Processings;

internal interface IPackageImportEventProcessingService
{
    ValueTask RaiseImportAsync<T>(
        string eventName,
        PackageItemImportEvent<T> import);
}