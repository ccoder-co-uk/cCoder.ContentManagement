// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Security;

namespace cCoder.ContentManagement.Models;

public sealed class TemplateRenderOperation
{
    public int AppId { get; set; }

    public string Name { get; set; }

    public object Model { get; set; }

    public User User { get; set; }

    public string Culture { get; set; }

    public Template Template { get; set; }

    public RenderParams RenderParams { get; set; }

    public string Result { get; set; }
}