// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data;
using cCoder.Data.Models.DMS;
using DmsFile = cCoder.Data.Models.DMS.File;
using Microsoft.EntityFrameworkCore;

namespace cCoder.ContentManagement.Brokers.Storages;

internal sealed class RenderFileContentBroker(ICoreContextFactory coreContextFactory) : IRenderFileContentBroker
{
    public byte[] GetLatestRawData(int appId, string path)
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();

        DmsFile file = coreDataContext
            .Set<DmsFile>()
            .AsNoTracking()
            .FirstOrDefault(predicate: foundFile => foundFile.Folder.AppId == appId && foundFile.Path == path);

        if (file == null)
        {
            return Array.Empty<byte>();
        }

        return coreDataContext.Set<FileContent>()
            .AsNoTracking()
            .Where(predicate: foundContent => foundContent.FileId == file.Id)
            .OrderByDescending(keySelector: foundContent => foundContent.Version)
            .Select(selector: foundContent => foundContent.RawData)
            .FirstOrDefault() ?? Array.Empty<byte>();
    }
}