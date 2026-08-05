// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Text.RegularExpressions;
using cCoder.ContentManagement.Models.PageRendering;

namespace cCoder.ContentManagement.Services.Processings.PageRendering;

internal sealed partial class ContentTagHandlingProcessingService
    : IContentTagHandlingProcessingService
{
    private static readonly Regex contentRegex = new(
        pattern: "\\[content\\[(?<name>[A-Za-z\\d_\\-/. ]+)\\](?<options>[^\\]]*)\\]",
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

        if (!operation.AllowContentTags)
        {
            return operation;
        }

        operation.Content = contentRegex.Replace(
            input: operation.Content,
            evaluator: match => ReplaceContentTag(
                operation: operation,
                match: match));

        return operation;
    });

    private static string ReplaceContentTag(
        TagHandlingOperation operation,
        Match match)
    {
        string name = match.Groups["name"].Value;

        string[] options = match.Groups["options"].Value
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