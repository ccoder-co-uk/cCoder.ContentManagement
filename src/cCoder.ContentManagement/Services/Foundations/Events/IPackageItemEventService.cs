// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Packaging;

namespace cCoder.ContentManagement.Services.Foundations.Events;

internal interface IPackageItemEventService
{
    ValueTask RaisePackageItemAddEventAsync(PackageItem entity, string userId);

    ValueTask RaisePackageItemUpdateEventAsync(PackageItem entity, string userId);

    ValueTask RaisePackageItemDeleteEventAsync(PackageItem entity, string userId);
}