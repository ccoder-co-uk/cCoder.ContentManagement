// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Security;

namespace cCoder.ContentManagement.Models;

public class AuthorizationContext
{
    public int AppId { get; set; }

    public AuthorizationRequest Request { get; set; }

    public PageAuthorization PageAuthorization { get; set; }

    public RenderAuthorization RenderAuthorization { get; set; }

    public User User { get; set; }

    public string UserId { get; set; }

    public string Culture { get; set; }
}