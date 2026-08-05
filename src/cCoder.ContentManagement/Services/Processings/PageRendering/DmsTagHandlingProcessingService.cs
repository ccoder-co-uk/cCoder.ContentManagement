// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Text;
using System.Text.RegularExpressions;
using cCoder.ContentManagement.Brokers.Storages;
using cCoder.ContentManagement.Models.PageRendering;

namespace cCoder.ContentManagement.Services.Processings.PageRendering;

internal sealed partial class DmsTagHandlingProcessingService(
    IRenderFileContentBroker renderFileContentBroker)
        : IDmsTagHandlingProcessingService
{
    private static readonly Regex dmsRegex = new(
        pattern: "\\[dms\\[(?<name>[A-Za-z\\d_\\-/. ]+)\\]\\]",
        options: RegexOptions.IgnoreCase
            | RegexOptions.Compiled
            | RegexOptions.Singleline);

    public TagHandlingOperation HandleTagHandlingOperation(
        TagHandlingOperation operation) =>
        TryCatch(operation: () =>
    {
        ValidateTagHandlingOperationOnHandle(inputs: [operation]);

        ValidateTagHandlingOperation(
            operation: operation,
            parameterName: "operation");

        operation.Content = dmsRegex.Replace(
            input: operation.Content,
            evaluator: match => ResolveContent(
                appId: operation.Session.App?.Id ?? 0,
                path: match.Groups["name"].Value));

        return operation;
    });

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