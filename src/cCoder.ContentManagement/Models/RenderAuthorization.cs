// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Security;

namespace cCoder.ContentManagement.Models;

public class RenderAuthorization
{
    public string Culture { get; set; }

    public User User { get; set; }
}