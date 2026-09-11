// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

namespace cCoder.ContentManagement.Models.PageRendering;

internal sealed class MarkupReplacement
{
    public string Old { get; set; }

    public string Value { get; set; }

    public Func<string, string> ReplaceFunction { get; set; }

    public string New => ReplaceFunction is null
        ? Value
        : ReplaceFunction(arg: Old);
}