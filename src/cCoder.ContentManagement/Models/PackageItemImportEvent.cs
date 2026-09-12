// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

namespace cCoder.ContentManagement.Models;

internal sealed class PackageItemImportEvent<T>
{
    public int? AppId { get; init; }

    public T[] Items { get; init; }
}