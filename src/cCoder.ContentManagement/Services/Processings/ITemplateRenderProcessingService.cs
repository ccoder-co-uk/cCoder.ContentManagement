// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Security;

namespace cCoder.ContentManagement.Services.Processings;

public interface ITemplateRenderProcessingService
{
    string RenderUser(int appId, string name, object model, User user, string culture);

    string RenderTemplateRenderParams(
        Template template,
        object model,
        RenderParams renderParams);
}
