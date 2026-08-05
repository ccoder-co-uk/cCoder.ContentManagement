// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

namespace cCoder.ContentManagement.Models;

public sealed class HttpPageRenderContext
{
    private string culture = string.Empty;
    private string theme = string.Empty;

    public string Domain { get; set; }

    public string Path { get; set; }

    public string Culture
    {
        get => culture;
        set => culture = (value ?? string.Empty)
            .Trim()
            .ToLowerInvariant();
    }

    public string Theme
    {
        get => theme;
        set => theme = (value ?? string.Empty)
            .Trim()
            .ToLowerInvariant();
    }

    public string Nonce { get; set; }

    public string RequestUrl { get; set; }

    public bool Edit { get; set; }

    public bool AccessDenied { get; set; }

    public int? PageId { get; set; }

    public int AppId { get; set; }

    public string TenantId { get; set; }

    public string AppConfigJson { get; set; }
}