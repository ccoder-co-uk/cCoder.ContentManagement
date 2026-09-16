// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Security;

namespace cCoder.ContentManagement.Models;

public sealed class AppManagerContext
{
    public int AppId { get; set; }

    public string Domain { get; set; }

    public bool IgnoreFilters { get; set; }

    public string UserName { get; set; }

    public App App { get; set; }

    public IQueryable<App> Apps { get; set; }

    public IQueryable<User> Users { get; set; }

    public bool IsAdmin { get; set; }
}