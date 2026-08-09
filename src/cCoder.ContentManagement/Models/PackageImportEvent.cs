// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Packaging;

namespace cCoder.ContentManagement.Models;

public sealed class PackageImportEvent
{
    public int AppId { get; set; }

    public Package Package { get; set; }
}