// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

namespace cCoder.ContentManagement.Models;

public class Replacement
{
    private readonly string newString;

    public string Old { get; }

    public string New =>
        newString ?? ReplaceFunction(arg: Old);

    public Func<string, string> ReplaceFunction { get; }
    public Replacement(string old, string @new)
    {
        this.ReplaceFunction = (string source) => source;
        Old = old;
        newString = @new;
    }

    public Replacement(string old, Func<string, string> replacer)
    {
        this.ReplaceFunction = (string source) => source;
        Old = old;

        if (replacer != null)
        {
            ReplaceFunction = replacer;
        }
    }
}