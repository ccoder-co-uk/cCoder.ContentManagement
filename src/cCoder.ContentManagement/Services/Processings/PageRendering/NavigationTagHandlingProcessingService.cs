// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Text.RegularExpressions;
using cCoder.ContentManagement.Models.PageRendering;

namespace cCoder.ContentManagement.Services.Processings.PageRendering;

internal sealed partial class NavigationTagHandlingProcessingService
    : INavigationTagHandlingProcessingService
{
    private static readonly Regex navigationRegex = new(
        pattern: "\\[nav\\[(?<name>[A-Za-z\\d_\\-/. ]*)\\]\\]",
        options: RegexOptions.IgnoreCase
            | RegexOptions.Compiled
            | RegexOptions.Singleline);

    private static readonly Regex expandedNavigationRegex = new(
        pattern: "\\[navExpanded\\[(?<name>[A-Za-z\\d_\\-/. ]*)\\]\\]",
        options: RegexOptions.IgnoreCase
            | RegexOptions.Compiled
            | RegexOptions.Singleline);

    public TagHandlingOperation HandleTagHandlingOperation(
        TagHandlingOperation operation) =>
        TryCatch(operation: () =>
    {
        ValidateTagHandlingOperationOnHandle(inputs: [operation]);

        ValidateTagHandlingOperation(
            operation: operation,
            parameterName: "operation");

        operation.Content = navigationRegex.Replace(
            input: operation.Content,
            evaluator: match => BuildMenu(
                session: operation.Session,
                tagName: match.Groups["name"].Value,
                expand: false));

        operation.Content = expandedNavigationRegex.Replace(
            input: operation.Content,
            evaluator: match => BuildMenu(
                session: operation.Session,
                tagName: match.Groups["name"].Value,
                expand: true));

        return operation;
    });

    private static string BuildMenu(
        PageRenderSession session,
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
        PageRenderSession session,
        PageRenderPage page,
        bool expand)
    {
        if (session.App is null)
        {
            return string.Empty;
        }

        return string.Join(
            separator: string.Empty,
            values: session.App.PagesById.Values
                .Where(predicate: subPage =>
                    subPage.ParentId == page?.Id
                    && subPage.ShowOnMenus)
                .OrderBy(keySelector: subPage => subPage.Order)
                .Select(selector: subPage => BuildMenuItem(
                    session: session,
                    parent: page,
                    page: subPage,
                    expand: expand)));
    }

    private static string BuildMenuItem(
        PageRenderSession session,
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