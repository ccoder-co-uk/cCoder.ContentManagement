using cCoder.Data;
using cCoder.Data.Models.DMS;
using Microsoft.EntityFrameworkCore;

namespace cCoder.ContentManagement.Brokers.Storages;

internal sealed class RenderFileContentBroker(ICoreContextFactory coreContextFactory) : IRenderFileContentBroker
{
    public byte[] GetLatestRawData(int appId, string path)
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();
        cCoder.Data.Models.DMS.File file = coreDataContext
            .Set<cCoder.Data.Models.DMS.File>()
            .AsNoTracking()
            .FirstOrDefault(foundFile => foundFile.Folder.AppId == appId && foundFile.Path == path);
        if (file == null)
        {
            return Array.Empty<byte>();
        }
        return (from foundContent in coreDataContext.Set<FileContent>().AsNoTracking()
                where foundContent.FileId == file.Id
                orderby foundContent.Version descending
                select foundContent.RawData).FirstOrDefault() ?? Array.Empty<byte>();
    }
}
