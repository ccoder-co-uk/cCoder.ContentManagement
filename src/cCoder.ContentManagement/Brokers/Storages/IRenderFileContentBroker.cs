namespace cCoder.ContentManagement.Brokers.Storages;

public interface IRenderFileContentBroker
{
    byte[] GetLatestRawData(int appId, string path);
}
