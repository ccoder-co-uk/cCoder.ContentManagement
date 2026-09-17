// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Text;
using cCoder.ContentManagement.Brokers;
using cCoder.ContentManagement.Brokers.Rendering;
using cCoder.ContentManagement.Brokers.Caching;
using cCoder.ContentManagement.Models.Caching;
using cCoder.ContentManagement.Models;
using cCoder.ContentManagement.Models.PageRendering;
using cCoder.ContentManagement.Models.Rendering;

namespace cCoder.ContentManagement.Rendering.Services.Foundations;

internal sealed partial class MarkupRenderService(
    IContentRenderBroker contentRenderBroker,
    IWorkflowExecutionBroker workflowExecutionBroker,
    IJsonBroker jsonBroker,
    IRegularExpressionBroker regularExpressionBroker,
    ICacheBroker cacheBroker,
    IRenderingUtilityBroker renderingUtilityBroker = null)
        : IMarkupRenderService
{
    private readonly IRenderingUtilityBroker renderingUtilityBroker =
        renderingUtilityBroker ?? new RenderingUtilityBroker();

    public TagHandlingOperation PrepareRenderSessionTagHandlingOperation(
        TagHandlingOperation tagHandlingOperation) =>
        TryCatch(operation: () =>
        {
            ValidateTagHandlingOperation(inputs: [tagHandlingOperation]);

            RenderSession renderSession = tagHandlingOperation.Session;

            string culture = !string.IsNullOrWhiteSpace(
                value: renderSession.Request.Culture)
                    ? renderSession.Request.Culture
                    : renderSession.App?.DefaultCulture ?? string.Empty;

            MetadataCacheSnapshot metadata = cacheBroker
                .Get<MetadataCacheSnapshot>(key: "ContentManagement.Metadata");

            renderSession.MetadataResolver = name =>
                metadata?.Serialized is not null
                && metadata.Serialized.TryGetValue(
                    key: culture,
                    value: out IDictionary<string, string> values)
                && values.TryGetValue(key: name, value: out string value)
                    ? value
                    : string.Empty;

            CommonObjectCacheSnapshot commonObjects = cacheBroker
                .Get<CommonObjectCacheSnapshot>(
                    key: "ContentManagement.CommonObjects");

            PopulateCommonObjects(
                renderSession: renderSession,
                commonObjects: commonObjects);

            tagHandlingOperation.Session = renderSession;
            return tagHandlingOperation;
        });

    public TagHandlingOperation HtmlEncodeTagHandlingOperation(
        TagHandlingOperation tagHandlingOperation) =>
        TryCatch(operation: () =>
        {
            ValidateHtmlEncodeTagHandlingOperation(inputs: [tagHandlingOperation]);

            tagHandlingOperation.Content = renderingUtilityBroker.HtmlEncode(
                value: tagHandlingOperation.Content);

            return tagHandlingOperation;
        });

    public TagHandlingOperation GetPropertyValuesTagHandlingOperation(
        TagHandlingOperation tagHandlingOperation) =>
        TryCatch(operation: () =>
        {
            ValidatePropertyValuesTagHandlingOperationOnGet(
                inputs: [tagHandlingOperation]);

            tagHandlingOperation.RuntimeProperties = renderingUtilityBroker
                .GetPropertyValues(value: tagHandlingOperation.Value);

            return tagHandlingOperation;
        });

    private const string ElementPattern = "<(?<tag>script|style)\\b";

    private const string NoncePattern =
        "\\s+nonce\\s*=\\s*(?:'[^']*'|\"[^\"]*\"|[^\\s>]+)";

    private const string NonceAttribute =
        "nonce='" + ContentSecurityPolicyNonceContract.Placeholder + "'";

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