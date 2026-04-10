using System.Text;
using cCoder.ContentManagement.Brokers.Storages;

namespace cCoder.ContentManagement.Services.Foundations;

internal partial class RenderFileContentService(IRenderFileContentBroker broker) : IRenderFileContentService
{
    public string GetLatestTextContent(int appId, string path)
    {
        ValidateAppId(appId, "appId");
        ValidatePath(path, "path");
        path = path?.ToLowerInvariant() ?? string.Empty;
        byte[] latestRawData = broker.GetLatestRawData(appId, path);
        return (latestRawData != null && latestRawData.Length != 0) ? Encoding.UTF8.GetString(latestRawData) : string.Empty;
    }
}
