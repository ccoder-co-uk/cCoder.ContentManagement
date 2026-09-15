// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Text;
using cCoder.ContentManagement.Exposures.Caching;
using cCoder.Data;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.DMS;
using Microsoft.EntityFrameworkCore;
using DmsFile = cCoder.Data.Models.DMS.File;

namespace cCoder.ContentManagement.Dependencies;

internal sealed class ContentRenderDependency(
    ICoreContextFactory coreContextFactory,
    ICommonObjectCache commonObjectCache,
    IMetadataCache metadataCache,
    WorkflowExecutionDependency workflowExecutionDependency)
{
    internal App[] GetApps()
    {
        using CoreDataContext coreDataContext =
            coreContextFactory.CreateCoreContext();

        return coreDataContext.Apps
            .IgnoreQueryFilters()
            .AsNoTracking()
            .ToArray();
    }

    internal Component[] GetComponents()
    {
        using CoreDataContext coreDataContext =
            coreContextFactory.CreateCoreContext();

        return coreDataContext.Components
            .IgnoreQueryFilters()
            .AsNoTracking()
            .ToArray();
    }

    internal Resource[] GetResources()
    {
        using CoreDataContext coreDataContext =
            coreContextFactory.CreateCoreContext();

        return coreDataContext.Resources
            .IgnoreQueryFilters()
            .AsNoTracking()
            .ToArray();
    }

    internal Script[] GetScripts()
    {
        using CoreDataContext coreDataContext =
            coreContextFactory.CreateCoreContext();

        return coreDataContext.Scripts
            .IgnoreQueryFilters()
            .AsNoTracking()
            .ToArray();
    }

    internal Template[] GetTemplates()
    {
        using CoreDataContext coreDataContext =
            coreContextFactory.CreateCoreContext();

        return coreDataContext.Templates
            .IgnoreQueryFilters()
            .AsNoTracking()
            .ToArray();
    }

    internal Component GetComponent(int appId, string name)
    {
        using CoreDataContext coreDataContext =
            coreContextFactory.CreateCoreContext();

        string lowerName = name.ToLowerInvariant();

        return coreDataContext.Components
            .IgnoreQueryFilters()
            .AsNoTracking()
            .FirstOrDefault(predicate: component =>
                component.AppId == appId
                && component.Name != null
                && component.Name.ToLower() == lowerName);
    }

    internal Script GetScript(int appId, string name)
    {
        using CoreDataContext coreDataContext =
            coreContextFactory.CreateCoreContext();

        string lowerName = name.ToLowerInvariant();

        return coreDataContext.Scripts
            .IgnoreQueryFilters()
            .AsNoTracking()
            .FirstOrDefault(predicate: script =>
                script.AppId == appId
                && script.Name != null
                && script.Name.ToLower() == lowerName);
    }

    internal T GetCommonObject<T>(string key) =>
        commonObjectCache.Get<T>(key: key);

    internal string GetMetadata(string key, string culture) =>
        metadataCache.Get(key: key, culture: culture);

    internal string GetLatestTextContent(int appId, string path)
    {
        using CoreDataContext coreDataContext =
            coreContextFactory.CreateCoreContext();

        byte[] rawData = coreDataContext.Set<DmsFile>()
            .AsNoTracking()
            .Where(predicate: foundFile =>
                foundFile.Folder.AppId == appId
                && foundFile.Path == path)
            .SelectMany(selector: foundFile =>
                coreDataContext.Set<FileContent>()
                    .Where(predicate: foundContent =>
                        foundContent.FileId == foundFile.Id))
            .OrderByDescending(keySelector: foundContent =>
                foundContent.Version)
            .Select(selector: foundContent => foundContent.RawData)
            .FirstOrDefault() ?? [];

        return Encoding.UTF8.GetString(bytes: rawData);
    }

    internal string ExecuteWorkflow(string baseAddress, string content) =>
        workflowExecutionDependency.Execute(
            baseAddress: baseAddress,
            content: content);
}