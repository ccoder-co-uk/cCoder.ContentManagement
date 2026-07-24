// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Security;

namespace cCoder.ContentManagement.Models;

public class TemplateRenderParams : RenderParams
{
    public TemplateRenderParams(App app, User user, string culture)
        : base(app, user, culture)
    {
    }
}