// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

namespace cCoder.ContentManagement.Models;

public class AuthorizationRequest
{
    public int? AppId { get; set; }

    public string Privilege { get; set; }

    public string UserName { get; set; }
}