// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System;
using System.Collections.Generic;
using cCoder.ContentManagement.Models.PageRendering;

namespace cCoder.ContentManagement.Rendering.Services.Foundations;

internal sealed partial class MarkupRenderService
{
    private const string ExecutePattern = "\\[execute\\](.*?)\\[/execute\\]";

    private TagHandlingOperation RenderExecuteTagHandlingOperationCore(
        TagHandlingOperation tagHandlingOperation)
    {

        tagHandlingOperation.Content = regularExpressionBroker.Replace(
            input: tagHandlingOperation.Content,
            pattern: ExecutePattern,
            evaluator: (value, groups) => Execute(
                code: groups["1"],
                replacements: tagHandlingOperation.Replacements));

        return tagHandlingOperation;
    }

    private string Execute(
        string code,
        IReadOnlyCollection<MarkupReplacement> replacements)
    {
        string json = "{}";
        string workflowBaseAddress = null;

        foreach (MarkupReplacement replacement in replacements)
        {
            if (replacement.Old == "[model]")
            {
                json = replacement.New;
            }
            else if (replacement.Old == "[api[workflow]]")
            {
                workflowBaseAddress = replacement.New;
            }
        }

        if (workflowBaseAddress is null)
        {
            throw new InvalidOperationException(
                message: "The workflow API replacement is required.");
        }

        string content = jsonBroker.SerializeIgnoringReferences(value: new
        {
            Script = code,
            Model = jsonBroker.ParseJson(json: json)
        });

        return workflowExecutionBroker.Execute(
            baseAddress: workflowBaseAddress,
            content: content);
    }

}