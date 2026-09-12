// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Models;

namespace cCoder.ContentManagement.Services.Foundations.Events;

internal interface IPackageImportEventService
{
    ValueTask RaiseImportAsync<T>(
        string eventName,
        PackageItemImportEvent<T> import);
}