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

        return coreDataContext.Set<DmsFile>()
            .AsNoTracking()
            .Where(predicate: foundFile => foundFile.Folder.AppId == appId && foundFile.Path == path)
            .SelectMany(
                selector: foundFile => coreDataContext.Set<FileContent>()
                    .Where(predicate: foundContent => foundContent.FileId == foundFile.Id))
            .OrderByDescending(keySelector: foundContent => foundContent.Version)
            .Select(selector: foundContent => foundContent.RawData)
            .FirstOrDefault() ?? Array.Empty<byte>();
    }
}