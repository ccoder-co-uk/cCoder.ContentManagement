// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Text.RegularExpressions;
using cCoder.ContentManagement.Brokers;
using cCoder.ContentManagement.Models.PageRendering;

namespace cCoder.ContentManagement.Services.Processings.PageRendering;

internal sealed partial class ExecuteTagHandlingProcessingService(
    IJsonBroker jsonBroker,
    IWorkflowExecutionBroker workflowExecutionBroker)
        : IExecuteTagHandlingProcessingService
{
    private static readonly Regex executeRegex = new(
        pattern: "\\[execute\\](.*?)\\[/execute\\]",
        options: RegexOptions.IgnoreCase
            | RegexOptions.Compiled
            | RegexOptions.Singleline);

    public TagHandlingOperation HandleTagHandlingOperation(
        TagHandlingOperation tagHandlingOperation) =>
        TryCatch(operation: () =>
    {
        ValidateTagHandlingOperationOnHandle(inputs: [tagHandlingOperation]);

        ValidateTagHandlingOperation(
            operation: tagHandlingOperation,
            parameterName: "operation");

        tagHandlingOperation.Content = executeRegex.Replace(
            input: tagHandlingOperation.Content,
            evaluator: match => Execute(
                code: match.Groups[1].Value,
                replacements: tagHandlingOperation.Replacements));

        return tagHandlingOperation;
    });

    private string Execute(
        string code,
        IReadOnlyCollection<ReplacementDependency> replacements)
    {
        string json = replacements
            .FirstOrDefault(predicate: replacement =>
                replacement.Old == "[model]")?.New ?? "{}";

        string content = jsonBroker.SerializeIgnoringReferences(value: new
        {
            Script = code,
            Model = jsonBroker.ParseJson(json: json)
        });

        return workflowExecutionBroker.Execute(
            baseAddress: replacements.First(predicate: replacement =>
                replacement.Old == "[api[workflow]]").New,
            content: content);
    }

}