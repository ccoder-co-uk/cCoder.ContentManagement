// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Net;
using System.Text;
using cCoder.Data;
using cCoder.ContentManagement.Models.Rendering;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.DMS;
using Microsoft.EntityFrameworkCore;
using DmsFile = cCoder.Data.Models.DMS.File;

namespace cCoder.ContentManagement.Brokers.Rendering;

internal sealed class ContentRenderBroker(
    ICoreContextFactory coreContextFactory)
        : IContentRenderBroker
{
    public App[] GetApps() =>
        Query(selectSet: context => context.Apps);

    public Component[] GetComponents() =>
        Query(selectSet: context => context.Components);

    public Resource[] GetResources() =>
        Query(selectSet: context => context.Resources);

    public Script[] GetScripts() =>
        Query(selectSet: context => context.Scripts);

    public Template[] GetTemplates() =>
        Query(selectSet: context => context.Templates);

    public Component GetComponent(int appId, string name)
    {
        using CoreDataContext context =
            coreContextFactory.CreateCoreContext();

        string lowerName = name.ToLowerInvariant();

        return context.Components
            .IgnoreQueryFilters()
            .AsNoTracking()
            .FirstOrDefault(predicate: component =>
                component.AppId == appId
                && component.Name != null
                && component.Name.ToLower() == lowerName);
    }

    public Script GetScript(int appId, string name)
    {
        using CoreDataContext context =
            coreContextFactory.CreateCoreContext();

        string lowerName = name.ToLowerInvariant();

        return context.Scripts
            .IgnoreQueryFilters()
            .AsNoTracking()
            .FirstOrDefault(predicate: script =>
                script.AppId == appId
                && script.Name != null
                && script.Name.ToLower() == lowerName);
    }

    public string GetLatestTextContent(int appId, string path)
    {
        using CoreDataContext context =
            coreContextFactory.CreateCoreContext();

        byte[] rawData = context.Set<DmsFile>()
            .AsNoTracking()
            .Where(predicate: file =>
                file.Folder.AppId == appId
                && file.Path == path)
            .SelectMany(selector: file => context.Set<FileContent>()
                .Where(predicate: content => content.FileId == file.Id))
            .OrderByDescending(keySelector: content => content.Version)
            .Select(selector: content => content.RawData)
            .FirstOrDefault() ?? [];

        return Encoding.UTF8.GetString(bytes: rawData);
    }

    public string HtmlEncode(string value) =>
        WebUtility.HtmlEncode(value: value);

    public RuntimePropertyValue[] GetPropertyValues(object value) =>
        value.GetType()
            .GetProperties()
            .Select(selector: property => new RuntimePropertyValue
            {
                Name = property.Name,
                Value = property.GetValue(obj: value),
                IsValueType = property.PropertyType.IsValueType
                    || property.PropertyType == typeof(string)
            })
            .ToArray();

    private T[] Query<T>(
        Func<CoreDataContext, DbSet<T>> selectSet)
        where T : class
    {
        using CoreDataContext context =
            coreContextFactory.CreateCoreContext();

        return selectSet(arg: context)
            .IgnoreQueryFilters()
            .AsNoTracking()
            .ToArray();
    }
}