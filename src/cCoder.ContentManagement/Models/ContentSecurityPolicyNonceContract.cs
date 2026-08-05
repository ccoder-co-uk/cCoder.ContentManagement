// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

namespace cCoder.ContentManagement.Models;

/// <summary>
/// Defines the placeholder replaced with a fresh content security policy nonce for each response.
/// </summary>
public static class ContentSecurityPolicyNonceContract
{
    public const string HttpContextItemKey =
        "cCoder.ContentManagement.PageNonce";

    public const string Placeholder = "[page[nonce]]";
}