// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

namespace cCoder.ContentManagement.Models;

using cCoder.Data.Models.CMS;

internal sealed class PageAuthorizationResult
{
    public int? PageId { get; set; }

    public string Layout { get; set; }

    public int AppId { get; set; }

    public string TenantId { get; set; }

    public string Domain { get; set; }

    public string DefaultCulture { get; set; }

    public string DefaultTheme { get; set; }

    public string AppConfigJson { get; set; }

    public PageRenderCache[] CacheCandidates { get; set; }

    public string UserId { get; set; }

    public string UserDefaultCultureId { get; set; }

    public string UserDisplayName { get; set; }

    public string UserEmail { get; set; }

    public bool? UserIsActive { get; set; }
}