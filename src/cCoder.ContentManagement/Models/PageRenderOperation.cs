// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Security;
using cCoder.Data.Models;

namespace cCoder.ContentManagement.Models;

public enum PageRenderOperationType
{
    Render,
    RenderError,
    RenderResult,
    UserCanPage
}

public class PageRenderOperation : PageRenderResponse
{
    internal bool RebuildCache { get; set; }
    internal bool CreateCache { get; set; }
    internal bool HeaderOnly { get; set; }
    internal bool CacheTemplate { get; set; }
    public PageRenderOperationType OperationType { get; set; }

    public string Host { get; set; }

    public string Path { get; set; }

    public string RequestUrl { get; set; }

    public Exception Exception { get; set; }

    public int AppId { get; set; }

    public int PageId { get; set; }

    public PageRenderCache[] PageRenderCaches { get; set; }

    public CommonObject CommonObject { get; set; }

    public Page SourcePage { get; set; }

    public User User { get; set; }

    public string Privilege { get; set; }

    public bool IsAuthorized { get; set; }

}