// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models;
using cCoder.Data.Models.CMS;

namespace cCoder.ContentManagement.Exposures.EventHandlers;

internal interface IPageRenderCacheEventHandlers
{
    ValueTask RebuildPageAsync(Page page);
    ValueTask DeletePageAsync(Page deletedPage);
    ValueTask RebuildAppAsync(App app);
    ValueTask RebuildAppAsync(int appId);
    ValueTask DeleteAppAsync(App deletedApp);
    ValueTask RebuildAppAsync(AppCulture appCulture);
    ValueTask RebuildAppAsync(Layout layout);
    ValueTask RebuildAppAsync(Template template);
    ValueTask RebuildAppAsync(Component component);
    ValueTask RebuildAppAsync(Resource resource);
    ValueTask RebuildAppAsync(Script script);
    ValueTask RebuildPageAsync(Content content);
    ValueTask RebuildPageAsync(PageInfo pageInfo);
    ValueTask RebuildCommonCacheConsumersAsync(CommonObject commonObject);
}