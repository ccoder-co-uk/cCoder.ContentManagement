// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

namespace cCoder.ContentManagement.Models.RegularExpressions;

internal sealed class RegularExpressionMatch
{
    public string Value { get; init; }

    public IReadOnlyDictionary<string, string> Groups { get; init; }
}