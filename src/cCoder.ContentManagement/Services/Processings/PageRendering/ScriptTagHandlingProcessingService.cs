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
            evaluator: match => ResolveScriptContent(
                session: operation.Session,
                name: match.Groups["name"].Value));

        return operation;
    });

    private string ResolveScriptContent(RenderSession session, string name)
    {
        PageRenderScript script = ResolveScript(session: session, name: name);

        return script is not null
            && session.EmittedScriptNames.Add(item: script.Name)
                ? script.Content ?? string.Empty
                : string.Empty;
    }

    private PageRenderScript ResolveScript(
        RenderSession session,
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