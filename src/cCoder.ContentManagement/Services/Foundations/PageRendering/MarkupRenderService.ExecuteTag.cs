// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Brokers;
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
        string json = replacements
            .FirstOrDefault(predicate: replacement =>
                replacement.Old == "[model]")?.New ?? "{}";

        string content = jsonBroker.SerializeIgnoringReferences(value: new
        {
            Script = code,
            Model = jsonBroker.ParseJson(json: json)
        });

        return contentRenderBroker.ExecuteWorkflow(
            baseAddress: replacements.First(predicate: replacement =>
                replacement.Old == "[api[workflow]]").New,
            content: content);
    }

}