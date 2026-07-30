// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Packaging;

namespace cCoder.ContentManagement.Services.Foundations.Events;

internal interface IPackageItemEventService
{
    ValueTask RaisePackageItemAddEventAsync(PackageItem entity);

    ValueTask RaisePackageItemUpdateEventAsync(PackageItem entity);

    ValueTask RaisePackageItemDeleteEventAsync(PackageItem entity);
}