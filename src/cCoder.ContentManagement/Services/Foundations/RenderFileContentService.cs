// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

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

        return broker.GetLatestTextContent(appId: appId, path: path)
            ?? string.Empty;

    });
}