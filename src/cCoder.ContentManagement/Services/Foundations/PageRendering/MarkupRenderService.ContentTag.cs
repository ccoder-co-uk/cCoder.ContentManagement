// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System;
using System.Collections.Generic;
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

        List<string> classes = [];
        List<string> attributes = [];

        foreach (string option in options)
        {
            if (option.StartsWith(value: "class="))
            {
                classes.Add(item: option.Replace(
                    oldValue: "class=",
                    newValue: string.Empty));
            }
            else
            {
                attributes.Add(item: option);
            }
        }

        string optionalClass = string.Join(separator: " ", values: classes);
        string otherOptions = string.Join(separator: " ", values: attributes);

        string contentEditable = operation.Editable
            ? "contenteditable"
            : string.Empty;

        return $"<section name='{name}' class='content {optionalClass}' data-id='{content.Id}' {contentEditable} {otherOptions}>\n                        {content.Html}\n                    </section>";
    }
}