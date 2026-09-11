// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Text;
using cCoder.ContentManagement.Brokers;
using cCoder.ContentManagement.Models;
using cCoder.ContentManagement.Models.PageRendering;
using cCoder.ContentManagement.Rendering.Brokers;
using cCoder.ContentManagement.Services.Processings.PageRendering;

namespace cCoder.ContentManagement.Rendering.Services.Foundations;

internal sealed partial class MarkupRenderService(
    IRenderBroker renderBroker,
    IRegularExpressionBroker regularExpressionBroker) : IMarkupRenderService
{
    private const string ElementPattern = "<(?<tag>script|style)\\b";

    private const string NoncePattern =
        "\\s+nonce\\s*=\\s*(?:'[^']*'|\"[^\"]*\"|[^\\s>]+)";

    private const string NonceAttribute =
        "nonce='" + ContentSecurityPolicyNonceContract.Placeholder + "'";

    public string RenderRenderSessionReplacementDependencies(
        string key,
        string content,
        RenderSession renderSession,
        IReadOnlyCollection<ReplacementDependency> replacements,
        bool allowContentTags = true)
        =>
        TryCatch(operation: () =>
    {
        ValidateRenderRenderSessionReplacementDependencies(
            inputs: [key, content, renderSession, replacements, allowContentTags]);

        if (string.IsNullOrEmpty(value: content))
        {
            return string.Empty;
        }

        TagHandlingOperation operation = HandleTags(
            operation: new TagHandlingOperation
            {
                Session = renderSession,
                ResourceKey = key,
                Content = content,
                AllowContentTags = allowContentTags,
                Editable = renderSession.Request.Edit,
                Replacements = replacements,
                Fragments = []
            });

        return operation.Content;
    });

    private TagHandlingOperation HandleTags(TagHandlingOperation operation)
    {
        ITagHandlingProcessingService[] handlers =
        [
            .. renderBroker.GetTagHandlers()
        ];

        HashSet<string> observedContent = new(
            comparer: StringComparer.Ordinal);

        for (int pass = 0; pass < 64; pass++)
        {
            string contentBeforePass = operation.Content;

            if (!observedContent.Add(item: contentBeforePass))
            {
                throw new InvalidOperationException(
                    message: "Tag rendering entered a replacement cycle.");
            }

            foreach (ITagHandlingProcessingService handler in handlers)
            {
                operation = handler.HandleTagHandlingOperation(
                    operation: operation);
            }

            foreach (TagHandlingFragment fragment in operation.Fragments)
            {
                TagHandlingOperation renderedFragment = HandleTags(
                    operation: fragment.Operation);

                operation.Content = operation.Content.Replace(
                    oldValue: fragment.Token,
                    newValue: renderedFragment.Content);
            }

            operation.Fragments.Clear();

            if (string.Equals(
                a: contentBeforePass,
                b: operation.Content,
                comparisonType: StringComparison.Ordinal))
            {
                return operation;
            }
        }

        throw new InvalidOperationException(
            message: "Tag rendering exceeded the maximum replacement passes.");
    }

    public string MarkContentSecurityPolicyNonce(string markup) =>
        TryCatch<string>(operation: () =>
    {
        ValidateMarkContentSecurityPolicyNonce(inputs: [markup]);
        return MarkContentSecurityPolicyNonceCore(markup: markup);
    });

    private string MarkContentSecurityPolicyNonceCore(string markup)
    {
        if (string.IsNullOrEmpty(value: markup))
        {
            return markup ?? string.Empty;
        }

        StringBuilder result = new(capacity: markup.Length + 64);
        int position = 0;

        while (TryFindElement(
            markup: markup,
            startIndex: position,
            tagName: out string tagName,
            openingStart: out int openingStart))
        {
            int openingEnd = FindTagEnd(
                markup: markup,
                startIndex: openingStart + tagName.Length + 1);

            if (openingEnd < 0)
            {
                break;
            }

            result.Append(
                value: markup,
                startIndex: position,
                count: openingStart - position);

            string openingTag = markup.Substring(
                startIndex: openingStart,
                length: openingEnd - openingStart + 1);

            result.Append(value: MarkOpeningTag(openingTag: openingTag));
            int contentStart = openingEnd + 1;

            int closingStart = markup.IndexOf(
                value: "</" + tagName,
                startIndex: contentStart,
                comparisonType: StringComparison.OrdinalIgnoreCase);

            if (closingStart < 0)
            {
                position = contentStart;
                continue;
            }

            result.Append(
                value: markup,
                startIndex: contentStart,
                count: closingStart - contentStart);

            position = closingStart;
        }

        result.Append(
            value: markup,
            startIndex: position,
            count: markup.Length - position);

        return result.ToString();
    }

    private string MarkOpeningTag(string openingTag)
    {
        string withoutNonce = regularExpressionBroker.Replace(
            input: openingTag,
            pattern: NoncePattern,
            replacement: string.Empty);

        int insertAt = withoutNonce.EndsWith(
            value: "/>",
            comparisonType: StringComparison.Ordinal)
                ? withoutNonce.Length - 2
                : withoutNonce.Length - 1;

        return withoutNonce.Insert(
            startIndex: insertAt,
            value: " " + NonceAttribute);
    }

    private bool TryFindElement(
        string markup,
        int startIndex,
        out string tagName,
        out int openingStart)
    {
        bool success = regularExpressionBroker.TryMatch(
            input: markup,
            pattern: ElementPattern,
            startIndex: startIndex,
            index: out int matchIndex,
            groups: out IReadOnlyDictionary<string, string> groups);

        tagName = success
            ? groups["tag"]
            : string.Empty;

        openingStart = success ? matchIndex : -1;

        return success;
    }

    private static int FindTagEnd(string markup, int startIndex)
    {
        char quote = '\0';

        for (int index = startIndex; index < markup.Length; index++)
        {
            char current = markup[index];

            if (quote == '\0' && (current == '\'' || current == '"'))
            {
                quote = current;
            }
            else if (quote == current)
            {
                quote = '\0';
            }
            else if (quote == '\0' && current == '>')
            {
                return index;
            }
        }

        return -1;
    }
}