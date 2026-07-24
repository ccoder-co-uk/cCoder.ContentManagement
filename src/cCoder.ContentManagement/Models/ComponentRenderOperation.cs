// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Security;

namespace cCoder.ContentManagement.Models;

public sealed class ComponentRenderOperation
{
    public int AppId { get; set; }

    public string Name { get; set; }

    public User User { get; set; }

    public string Culture { get; set; }

    public string Theme { get; set; }

    public Component Component { get; set; }

    public ComponentRenderParams RenderParams { get; set; }

    public string Result { get; set; }
}