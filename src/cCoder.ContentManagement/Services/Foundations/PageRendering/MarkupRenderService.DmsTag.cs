// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Text;
using cCoder.ContentManagement.Brokers;
using cCoder.ContentManagement.Brokers.Storages;
using cCoder.ContentManagement.Models.PageRendering;

namespace cCoder.ContentManagement.Rendering.Services.Foundations;

internal sealed partial class MarkupRenderService
{
    private const string DmsPattern =
        "\\[dms\\[(?<name>[A-Za-z\\d_\\-/. ]+)\\]\\]";

    private TagHandlingOperation RenderDmsTagHandlingOperationCore(
        TagHandlingOperation tagHandlingOperation)
    {

        tagHandlingOperation.Content = regularExpressionBroker.Replace(
            input: tagHandlingOperation.Content,
            pattern: DmsPattern,
            evaluator: (value, groups) => ResolveContent(
                appId: tagHandlingOperation.Session.App?.Id ?? 0,
                path: groups["name"]));

        return tagHandlingOperation;
    }

    private string ResolveContent(int appId, string path)
    {
        byte[] latestRawData = renderFileContentBroker.GetLatestRawData(
            appId: appId,
            path: path);

        return latestRawData?.Length > 0
            ? Encoding.UTF8.GetString(bytes: latestRawData)
            : string.Empty;
    }
}