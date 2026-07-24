// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Security;

namespace cCoder.ContentManagement.Services.Orchestrations;

public interface IPageRenderOrchestrationService
{
    bool IsAdminOfApp(int appId);

    string ResolveCulture(string culture);

    bool UserCanPage(Page page, string privilege);

    RenderResult RenderPageRenderResult(
        Page page,
        string theme,
        string culture,
        bool edit = false);

    RenderResult RenderPageUserRenderResult(Page page, User user, string theme, string culture, bool edit = false);
}