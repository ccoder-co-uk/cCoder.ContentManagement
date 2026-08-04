// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

namespace cCoder.ContentManagement.Dependencies;

public class ReplacementDependency
{
    private readonly string newString;

    public string Old { get; }

    public string New =>
        newString ?? ReplaceFunction(arg: Old);

    public Func<string, string> ReplaceFunction { get; }

    public ReplacementDependency(string old, string @new)
    {
        ReplaceFunction = source => source;
        Old = old;
        newString = @new;
    }

    public ReplacementDependency(string old, Func<string, string> replacer)
    {
        ReplaceFunction = replacer ?? (source => source);
        Old = old;
    }
}