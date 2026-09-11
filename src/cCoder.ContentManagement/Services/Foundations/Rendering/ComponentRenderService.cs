// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Text;
using cCoder.ContentManagement.Brokers.Storages;

namespace cCoder.ContentManagement.Services.Foundations.Rendering;

internal sealed partial class ComponentRenderService(
    IRenderFileContentBroker renderFileContentBroker)
        : IComponentRenderService
{
    public string GetLatestTextContent(int appId, string path) =>
        TryCatch<string>(operation: () =>
    {
        ValidateLatestTextContentOnGet(inputs: [appId, path]);
        ValidateAppId(appId: appId, parameterName: "appId");
        ValidatePath(path: path, parameterName: "path");

        byte[] latestRawData = renderFileContentBroker.GetLatestRawData(
            appId: appId,
            path: path);

        return latestRawData is { Length: > 0 }
            ? Encoding.UTF8.GetString(bytes: latestRawData)
            : string.Empty;
    });
}