// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

namespace cCoder.ContentManagement.Brokers;

internal interface IRegularExpressionBroker
{
    string Replace(
        string input,
        string pattern,
        Func<string, IReadOnlyDictionary<string, string>, string> evaluator);

    string Replace(
        string input,
        string pattern,
        string replacement);

    bool TryMatch(
        string input,
        string pattern,
        int startIndex,
        out int index,
        out IReadOnlyDictionary<string, string> groups);

    void ForEachMatch(
        string input,
        string pattern,
        Action<string, IReadOnlyDictionary<string, string>> action);
}