// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Security;

namespace cCoder.ContentManagement.Models;

public abstract class RenderParams
{
    public App App { get; }

    public User User { get; }

    public string Culture { get; set; }

    protected RenderParams(App app, User user)
        : this(app, user, "")
    {
    }

    protected RenderParams(App app, User user, string culture)
    {
        App = app;
        User = user;
        Culture = culture;
    }
}