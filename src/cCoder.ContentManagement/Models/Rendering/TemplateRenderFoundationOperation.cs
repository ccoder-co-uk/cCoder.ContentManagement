// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.CMS;
using Microsoft.Extensions.Logging;

namespace cCoder.ContentManagement.Models.Rendering;

internal sealed class TemplateRenderFoundationOperation
{
    public string Key { get; set; }

    public string Culture { get; set; }

    public string Input { get; set; }

    public string Pattern { get; set; }

    public string Replacement { get; set; }

    public string BaseAddress { get; set; }

    public string Content { get; set; }

    public string Message { get; set; }

    public object[] Arguments { get; set; }

    public object Value { get; set; }

    public bool Condition { get; set; }

    public LogLevel LogLevel { get; set; }

    public IReadOnlyCollection<App> Apps { get; set; }
    public IReadOnlyCollection<Component> Components { get; set; }
    public IReadOnlyCollection<Resource> Resources { get; set; }
    public IReadOnlyCollection<Script> Scripts { get; set; }
    public IReadOnlyCollection<Template> Templates { get; set; }

    public Component Component { get; set; }

    public Script Script { get; set; }

    public Resource Resource { get; set; }

    public IReadOnlyCollection<KeyValuePair<string, object>> JsonProperties { get; set; }

    public Func<string, IReadOnlyDictionary<string, string>, string> Evaluator { get; set; }

    public Action<string, IReadOnlyDictionary<string, string>> MatchAction { get; set; }
}