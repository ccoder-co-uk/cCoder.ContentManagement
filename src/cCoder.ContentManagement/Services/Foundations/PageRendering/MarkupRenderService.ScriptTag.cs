// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Brokers;
using cCoder.ContentManagement.Models.PageRendering;
using cCoder.ContentManagement.Rendering.Brokers;
using cCoder.Data.Models.CMS;

namespace cCoder.ContentManagement.Rendering.Services.Foundations;

internal sealed partial class MarkupRenderService
{
    private const string ScriptPattern =
        "\\[script\\[(?<name>[A-Za-z\\d_\\-/. ]+)\\]\\]";

    private TagHandlingOperation RenderScriptTagHandlingOperationCore(
        TagHandlingOperation tagHandlingOperation)
    {

        tagHandlingOperation.Content = regularExpressionBroker.Replace(
            input: tagHandlingOperation.Content,
            pattern: ScriptPattern,
            evaluator: (value, groups) => ResolveScriptContent(
                session: tagHandlingOperation.Session,
                name: groups["name"]));

        return tagHandlingOperation;
    }

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

        Script dataScript = contentRenderBroker.GetScript(
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