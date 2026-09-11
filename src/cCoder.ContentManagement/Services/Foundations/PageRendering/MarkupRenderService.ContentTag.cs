// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Brokers;
using cCoder.ContentManagement.Models.PageRendering;
using cCoder.ContentManagement.Models.RegularExpressions;

namespace cCoder.ContentManagement.Rendering.Services.Foundations;

internal sealed partial class MarkupRenderService
{
    private const string ContentPattern =
        "\\[content\\[(?<name>[A-Za-z\\d_\\-/. ]+)\\](?<options>[^\\]]*)\\]";

    private TagHandlingOperation RenderContentTagHandlingOperationCore(
        TagHandlingOperation tagHandlingOperation)
    {

        if (!tagHandlingOperation.AllowContentTags)
        {
            return tagHandlingOperation;
        }

        tagHandlingOperation.Content = regularExpressionBroker.Replace(
            input: tagHandlingOperation.Content,
            pattern: ContentPattern,
            evaluator: (value, groups) => ReplaceContentTag(
                operation: tagHandlingOperation,
                match: new RegularExpressionMatch
                {
                    Value = value,
                    Groups = groups
                }));

        return tagHandlingOperation;
    }

    private static string ReplaceContentTag(
        TagHandlingOperation operation,
        RegularExpressionMatch match)
    {
        string name = match.Groups["name"];

        string[] options = match.Groups["options"]
            .Split(
                separator: ' ',
                options: StringSplitOptions.RemoveEmptyEntries);

        PageRenderContent content = null;

        operation.Session.Page?.ContentByName.TryGetValue(
            key: name,
            value: out content);

        if (content is null)
        {
            return "[[Missing Content:" + name + "]]";
        }

        string optionalClass = string.Join(
            separator: " ",
            values: options
                .Where(predicate: option => option.StartsWith(value: "class="))
                .Select(selector: option => option.Replace(
                    oldValue: "class=",
                    newValue: string.Empty)));

        string otherOptions = string.Join(
            separator: " ",
            values: options.Where(predicate: option =>
                !option.StartsWith(value: "class=")));

        string contentEditable = operation.Editable
            ? "contenteditable"
            : string.Empty;

        return $"<section name='{name}' class='content {optionalClass}' data-id='{content.Id}' {contentEditable} {otherOptions}>\n                        {content.Html}\n                    </section>";
    }
}