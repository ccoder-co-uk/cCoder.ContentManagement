// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Security;

namespace cCoder.ContentManagement.Models;

internal sealed class PageAuthorizationData
{
    public PageAuthorizationResult Result { get; set; }

    public User User { get; set; }
}