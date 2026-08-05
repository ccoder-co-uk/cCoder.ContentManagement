// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

namespace cCoder.ContentManagement.Models;

public sealed class HttpPageRenderContext
{
    public string Domain { get; set; }

    public string Path { get; set; }

    public string Culture { get; set; }

    public string Theme { get; set; }

    public string Nonce { get; set; }

    public string RequestUrl { get; set; }

    public bool Edit { get; set; }

    public int? PageId { get; set; }
}