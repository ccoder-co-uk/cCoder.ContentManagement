// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Brokers;
using cCoder.ContentManagement.Models.PageRendering;

namespace cCoder.ContentManagement.Rendering.Services.Foundations;

internal sealed partial class MarkupRenderService
{
    private const string NavigationPattern =
        "\\[nav\\[(?<name>[A-Za-z\\d_\\-/. ]*)\\]\\]";

    private const string ExpandedNavigationPattern =
        "\\[navExpanded\\[(?<name>[A-Za-z\\d_\\-/. ]*)\\]\\]";

    private TagHandlingOperation RenderNavigationTagHandlingOperationCore(
        TagHandlingOperation tagHandlingOperation)
    {

        tagHandlingOperation.Content = regularExpressionBroker.Replace(
            input: tagHandlingOperation.Content,
            pattern: NavigationPattern,
            evaluator: (value, groups) => BuildMenu(
                session: tagHandlingOperation.Session,
                tagName: groups["name"],
                expand: false));

        tagHandlingOperation.Content = regularExpressionBroker.Replace(
            input: tagHandlingOperation.Content,
            pattern: ExpandedNavigationPattern,
            evaluator: (value, groups) => BuildMenu(
                session: tagHandlingOperation.Session,
                tagName: groups["name"],
                expand: true));

        return tagHandlingOperation;
    }

    private static string BuildMenu(
        RenderSession session,
        string tagName,
        bool expand)
    {
        PageRenderPage page = null;

        if (int.TryParse(s: tagName, result: out int pageId)
            && session.App is not null)
        {
            session.App.PagesById.TryGetValue(
                key: pageId,
                value: out page);
        }

        return "<div class='collapse navbar-collapse'><ul class='navbar-nav'>"
            + BuildMenuItems(session: session, page: page, expand: expand)
            + "</ul></div>";
    }

    private static string BuildMenuItems(
        RenderSession session,
        PageRenderPage page,
        bool expand)
    {
        if (session.App is null)
        {
            return string.Empty;
        }

        List<PageRenderPage> pages = [];

        foreach (PageRenderPage subPage in session.App.PagesById.Values)
        {
            if (subPage.ParentId == page?.Id && subPage.ShowOnMenus)
            {
                pages.Add(item: subPage);
            }
        }

        pages.Sort(comparison: (left, right) => left.Order.CompareTo(
            value: right.Order));

        List<string> menuItems = new(capacity: pages.Count);

        foreach (PageRenderPage subPage in pages)
        {
            menuItems.Add(item: BuildMenuItem(
                session: session,
                parent: page,
                page: subPage,
                expand: expand));
        }

        return string.Concat(values: menuItems);
    }

    private static string BuildMenuItem(
        RenderSession session,
        PageRenderPage parent,
        PageRenderPage page,
        bool expand)
    {
        string selected = page.ParentId.HasValue
            && parent is not null
            && !string.IsNullOrWhiteSpace(value: session.Page?.Path)
            && session.Page.Path.Contains(value: page.Path)
                ? " active"
                : string.Empty;

        return expand
            ? $"<li data-id='{page.Id}' class='nav-item'><a href='/{page.Path}' class='nav-link{selected}'>{page.Title}</a><ul class='submenu dropdown-menu'>{BuildMenuItems(session: session, page: page, expand: true)}</ul></li>"
            : $"<li data-id='{page.Id}' class='nav-item'><a href='/{page.Path}' class='nav-link{selected}'>{page.Title}</a></li>";
    }
}