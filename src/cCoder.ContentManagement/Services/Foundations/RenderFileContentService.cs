// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Text;
using cCoder.ContentManagement.Brokers.Storages;

namespace cCoder.ContentManagement.Services.Foundations;

internal partial class RenderFileContentService(IRenderFileContentBroker broker) : IRenderFileContentService
{
    public string GetLatestTextContent(int appId, string path) =>
        TryCatch<string>(operation: () =>
    {
        ValidateLatestTextContentOnGet(inputs: [appId, path]);
        ValidateAppId(appId: appId, parameterName: "appId");
        ValidatePath(path: path, parameterName: "path");
        path = path?.ToLowerInvariant() ?? string.Empty;
        byte[] latestRawData = broker.GetLatestRawData(appId: appId, path: path);
        return (latestRawData != null && latestRawData.Length != 0) ? Encoding.UTF8.GetString(bytes: latestRawData) : string.Empty;

    });
}