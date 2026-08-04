// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

namespace cCoder.ContentManagement.Models.PageRendering;

internal sealed class PageRenderUser
{
    public string Id { get; set; }
    public string DefaultCultureId { get; set; }
    public string DisplayName { get; set; }
    public string Email { get; set; }
    public IReadOnlyDictionary<int, ISet<string>> AppPrivileges { get; set; }
}