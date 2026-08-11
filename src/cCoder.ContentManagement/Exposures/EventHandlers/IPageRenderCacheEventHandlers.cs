// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models;
using cCoder.Data.Models.CMS;

namespace cCoder.ContentManagement.Exposures.EventHandlers;

internal interface IPageRenderCacheEventHandlers
{
    ValueTask InvalidatePageAsync(Page page);
    ValueTask DeletePageAsync(Page deletedPage);
    ValueTask InvalidateAppAsync(App app);
    ValueTask InvalidateAppAsync(int appId);
    ValueTask DeleteAppAsync(App deletedApp);
    ValueTask InvalidateAppAsync(AppCulture appCulture);
    ValueTask InvalidateAppAsync(Layout layout);
    ValueTask InvalidateAppAsync(Template template);
    ValueTask InvalidateAppAsync(Component component);
    ValueTask InvalidateAppAsync(Resource resource);
    ValueTask InvalidateAppAsync(Script script);
    ValueTask InvalidatePageAsync(Content content);
    ValueTask InvalidatePageAsync(PageInfo pageInfo);
    ValueTask InvalidateCommonCacheConsumersAsync(CommonObject commonObject);
    ValueTask InvalidateCommonObjectsAsync(
        CommonObject[] commonObjects);
}