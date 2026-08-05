// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Text.RegularExpressions;
using cCoder.ContentManagement.Models.PageRendering;
using cCoder.ContentManagement.Rendering.Brokers;
using cCoder.Data.Models.CMS;

namespace cCoder.ContentManagement.Services.Processings.PageRendering;

internal sealed partial class ScriptTagHandlingProcessingService(
    IScriptReaderBroker scriptReaderBroker)
        : IScriptTagHandlingProcessingService
{
    private static readonly Regex scriptRegex = new(
        pattern: "\\[script\\[(?<name>[A-Za-z\\d_\\-/. ]+)\\]\\]",
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

        operation.Content = scriptRegex.Replace(
            input: operation.Content,
            evaluator: match => ResolveScript(
                session: operation.Session,
                name: match.Groups["name"].Value)?.Content ?? string.Empty);

        return operation;
    });

    private PageRenderScript ResolveScript(
        PageRenderSession session,
        string name)
    {
        if (session.ScriptsByName.TryGetValue(
            key: name,
            value: out PageRenderScript script))
        {
            return script;
        }

        Script dataScript = scriptReaderBroker.GetScript(
            appId: session.Request.AppId,
            name: name);

        if (dataScript is not null)
        {
            script = new PageRenderScript
            {
                Name = dataScript.Name ?? string.Empty,
                Content = dataScript.Content ?? string.Empty
            };

            session.ScriptsByName[name] = script;
            return script;
        }

        return session.CommonScriptsByName.TryGetValue(
            key: name,
            value: out script)
                ? script
                : null;
    }
}