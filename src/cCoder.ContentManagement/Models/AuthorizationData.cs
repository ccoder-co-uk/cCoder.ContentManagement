// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Security;

namespace cCoder.ContentManagement.Models;

internal sealed class AuthorizationData
{
    internal App App { get; init; }

    internal Role[] Roles { get; init; }

    internal User User { get; init; }

    internal string UserId { get; init; }
}