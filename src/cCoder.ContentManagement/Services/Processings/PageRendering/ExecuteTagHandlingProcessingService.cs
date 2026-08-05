// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Text.RegularExpressions;
using cCoder.ContentManagement.Brokers;
using cCoder.ContentManagement.Models.PageRendering;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

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
        TagHandlingOperation operation) =>
        TryCatch(operation: () =>
    {
        ValidateTagHandlingOperationOnHandle(inputs: [operation]);

        ValidateTagHandlingOperation(
            operation: operation,
            parameterName: "operation");

        operation.Content = executeRegex.Replace(
            input: operation.Content,
            evaluator: match => Execute(
                code: match.Groups[1].Value,
                replacements: operation.Replacements));

        return operation;
    });

    private string Execute(
        string code,
        IReadOnlyCollection<ReplacementDependency> replacements)
    {
        string json = replacements
            .FirstOrDefault(predicate: replacement =>
                replacement.Old == "[model]")?.New ?? "{}";

        string content = SerializeForOData(model: new
        {
            Script = code,
            Model = jsonBroker.ParseJson(json: json)
        });

        return workflowExecutionBroker.Execute(
            baseAddress: replacements.First(predicate: replacement =>
                replacement.Old == "[api[workflow]]").New,
            content: content);
    }

    private static string SerializeForOData(object model) =>
        JsonConvert.SerializeObject(
            value: model,
            formatting: Formatting.None,
            settings: new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                TypeNameHandling = TypeNameHandling.None,
                Formatting = Formatting.None,
                DateFormatHandling = DateFormatHandling.IsoDateFormat,
                NullValueHandling = NullValueHandling.Ignore,
                DateTimeZoneHandling = DateTimeZoneHandling.Utc,
                ContractResolver = new DefaultContractResolver
                {
                    IgnoreSerializableAttribute = true
                },
                MaxDepth = 4
            });
}