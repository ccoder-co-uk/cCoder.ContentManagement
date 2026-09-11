// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Text.RegularExpressions;

namespace cCoder.ContentManagement.Brokers;

internal sealed class RegularExpressionBroker : IRegularExpressionBroker
{
    private const RegexOptions StandardOptions =
        RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Singleline;

    private const RegexOptions MarkupOptions =
        RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.CultureInvariant;

    public string Replace(
        string input,
        string pattern,
        Func<string, IReadOnlyDictionary<string, string>, string> evaluator)
    {
        Regex expression = new(pattern: pattern, options: StandardOptions);

        return expression.Replace(
            input: input,
            evaluator: match => evaluator(
                arg1: match.Value,
                arg2: GetGroups(expression: expression, match: match)));
    }

    public string Replace(
        string input,
        string pattern,
        string replacement) =>
        new Regex(pattern: pattern, options: MarkupOptions)
            .Replace(input: input, replacement: replacement);

    public bool TryMatch(
        string input,
        string pattern,
        int startIndex,
        out int index,
        out IReadOnlyDictionary<string, string> groups)
    {
        Regex expression = new(pattern: pattern, options: MarkupOptions);
        Match match = expression.Match(input: input, startat: startIndex);

        index = match.Index;
        groups = GetGroups(expression: expression, match: match);

        return match.Success;
    }

    public void ForEachMatch(
        string input,
        string pattern,
        Action<string, IReadOnlyDictionary<string, string>> action)
    {
        Regex expression = new(pattern: pattern, options: StandardOptions);

        expression.Matches(input: input)
            .Cast<Match>()
            .ToList()
            .ForEach(action: match => action(
                arg1: match.Value,
                arg2: GetGroups(expression: expression, match: match)));
    }

    private static IReadOnlyDictionary<string, string> GetGroups(
        Regex expression,
        Match match) =>
        expression.GetGroupNames()
            .Select(selector: name => new KeyValuePair<string, string>(
                key: name,
                value: match.Groups[name].Value))
            .Concat(second: expression.GetGroupNumbers()
                .Select(selector: number => new KeyValuePair<string, string>(
                    key: number.ToString(provider: null),
                    value: match.Groups[number].Value)))
            .GroupBy(keySelector: group => group.Key)
            .ToDictionary(
                keySelector: group => group.Key,
                elementSelector: group => group.First().Value,
                comparer: StringComparer.Ordinal);
}