// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Security;

namespace cCoder.ContentManagement.Models;

public class PageAuthorization
{
    public Page Page { get; set; }

    public User User { get; set; }

    public string Privilege { get; set; }
}