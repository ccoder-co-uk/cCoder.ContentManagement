// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Security;
using cCoder.ContentManagement.Brokers;
using cCoder.ContentManagement.Brokers.Storages;
using cCoder.Data.Extensions;
using Newtonsoft.Json;
using cCoder.Data.Models.Packaging;

namespace cCoder.ContentManagement.Services.Foundations.Exports;

internal partial class PackageExportService(
    IAuthorizationBroker authorizationBroker,
    IRoleBroker roleBroker,
    ILayoutBroker layoutBroker,
    ITemplateBroker templateBroker,
    IComponentBroker componentBroker,
    IScriptBroker scriptBroker,
    IResourceBroker resourceBroker,
    IPageBroker pageBroker) : IPackageExportService
{
    private sealed class ExportPage
    {
        public int Id { get; init; }
        public int? ParentId { get; init; }
        public string Path { get; set; }
        public string Name { get; init; }
        public string ResourceKey { get; init; }
        public bool ShowOnMenus { get; init; }
        public int Order { get; init; }
        public DateTimeOffset LastUpdated { get; init; }
        public string Layout { get; init; }
        public ExportContent[] Contents { get; init; }
        public ExportPageInfo[] PageInfo { get; init; }
    }

    private sealed class ExportContent
    {
        public string CultureId { get; init; }
        public string Name { get; init; }
        public string Html { get; init; }
    }

    private sealed class ExportPageInfo
    {
        public string CultureId { get; init; }
        public string Description { get; init; }
        public string Keywords { get; init; }
        public string Title { get; init; }
    }

    public Package ExportRoles(int appId)
    {
        EnsureAdmin(appId: ValidateAppId(appId: appId, parameterName: "appId"));

        return CreatePackage(
name: "Roles",
itemType: "Core/Role",
data: roleBroker.GetAllRoles(ignoreFilters: true)
            .Where(predicate: role => role.AppId == appId)
            .Select(selector: role => new { role.Name, role.Privs })
            .ToArray());
    }

    public Package ExportLayouts(int appId)
    {
        EnsureAdmin(appId: ValidateAppId(appId: appId, parameterName: "appId"));

        return CreatePackage(
name: "Layouts",
itemType: "Core/Layout",
data: layoutBroker.GetAllLayouts(ignoreFilters: true)
            .Where(predicate: layout => layout.AppId == appId)
            .Select(selector: layout => new
            {
                layout.Name,
                layout.HeaderHtml,
                layout.Html,
                layout.Script,
                layout.LastUpdated
            })
            .ToArray());
    }

    public Package ExportTemplates(int appId)
    {
        EnsureAdmin(appId: ValidateAppId(appId: appId, parameterName: "appId"));

        return CreatePackage(
name: "Templates",
itemType: "Core/Template",
data: templateBroker.GetAllTemplates(ignoreFilters: true)
            .Where(predicate: template => template.AppId == appId)
            .Select(selector: template => new
            {
                template.Name,
                template.ResourceKey,
                template.RawString,
                template.LastUpdated
            })
            .ToArray());
    }

    public Package ExportComponents(int appId)
    {
        EnsureAdmin(appId: ValidateAppId(appId: appId, parameterName: "appId"));

        return CreatePackage(
name: "Components",
itemType: "Core/Component",
data: componentBroker.GetAllComponents(ignoreFilters: true)
            .Where(predicate: component => component.AppId == appId)
            .Select(selector: component => new
            {
                component.Name,
                component.Key,
                component.ResourceKey,
                component.Script,
                component.Content,
                component.LastUpdated
            })
            .ToArray());
    }

    public Package ExportScripts(int appId)
    {
        EnsureAdmin(appId: ValidateAppId(appId: appId, parameterName: "appId"));

        return CreatePackage(
name: "Scripts",
itemType: "Core/Script",
data: scriptBroker.GetAllScripts(ignoreFilters: true)
            .Where(predicate: script => script.AppId == appId)
            .Select(selector: script => new
            {
                script.Name,
                script.Content,
                script.LastUpdated
            })
            .ToArray());
    }

    public Package ExportResources(int appId)
    {
        EnsureAdmin(appId: ValidateAppId(appId: appId, parameterName: "appId"));

        return CreatePackage(
name: "Resources",
itemType: "Core/Resource",
data: resourceBroker.GetAllResources(ignoreFilters: true)
            .Where(predicate: resource => resource.AppId == appId)
            .Select(selector: resource => new
            {
                resource.Culture,
                resource.Key,
                resource.Name,
                resource.DisplayName,
                resource.ShortDisplayName,
                resource.Description,
                resource.LastUpdated
            })
            .ToArray());
    }

    public Package ExportPages(int appId)
    {
        EnsureAdmin(appId: ValidateAppId(appId: appId, parameterName: "appId"));

        List<ExportPage> pages = pageBroker.GetAllPages(ignoreFilters: true)
            .Where(predicate: page => page.AppId == appId)
            .Select(selector: page => new ExportPage
            {
                Id = page.Id,
                ParentId = page.ParentId,
                Path = page.Path,
                Name = page.Name,
                ResourceKey = page.ResourceKey,
                ShowOnMenus = page.ShowOnMenus,
                Order = page.Order,
                LastUpdated = page.LastUpdated,
                Layout = page.Layout,
                Contents = page.Contents
                    .Select(selector: content => new ExportContent
                    {
                        CultureId = content.CultureId,
                        Name = content.Name,
                        Html = content.Html
                    })
            .ToArray(),
                PageInfo = page.PageInfo
                    .Select(selector: info => new ExportPageInfo
                    {
                        CultureId = info.CultureId,
                        Description = info.Description,
                        Keywords = info.Keywords,
                        Title = info.Title
                    })
            .ToArray()
            })
            .ToList();

        Dictionary<int, ExportPage> pagesById = pages.ToDictionary(keySelector: page => page.Id);

        foreach (ExportPage page in pages.Where(predicate: page => page.ParentId.HasValue))
        {
            ExportPage root = page;

            while (root.ParentId.HasValue && pagesById.TryGetValue(key: root.ParentId.Value, value: out ExportPage parent))
            {
                root = parent;
            }

            if (string.IsNullOrEmpty(value: root.Path) && !string.IsNullOrEmpty(value: page.Path))
            {
                page.Path = "/" + page.Path.TrimStart(trimChar: '/');
            }
        }

        return CreatePackage(
name: "Pages",
itemType: "Core/Page",
data: pages.Select(selector: page => new
{
    page.Path,
    page.Name,
    page.ResourceKey,
    page.ShowOnMenus,
    page.Order,
    page.LastUpdated,
    page.Layout,
    page.Contents,
    page.PageInfo
})
            .ToArray());
    }

    public Package ExportPageRoles(int appId)
    {
        EnsureAdmin(appId: ValidateAppId(appId: appId, parameterName: "appId"));

        return CreatePackage(
name: "PageRoles",
itemType: "Core/PageRole",
data: pageBroker.GetAllPages(ignoreFilters: true)
            .Where(predicate: page => page.AppId == appId)
            .SelectMany(selector: page => page.Roles.Select(selector: role => new
            {
                page.Path,
                Role = role.Role.Name
            }))
            .ToArray());
    }

    private Package CreatePackage(string name, string itemType, object data)
    {
        return new Package(name: name)
        {
            Items =
            [
                new PackageItem
                {
                    Type = itemType,
                    Data = JsonConvert.SerializeObject(value: data, settings: CreateSerializerSettings())
                }
            ]
        };
    }

    private void EnsureAdmin(int appId)
    {
        if (!authorizationBroker.IsAdminOfApp(appId: appId))
        {
            throw new SecurityException(message: "Access Denied!");
        }
    }

    private static JsonSerializerSettings CreateSerializerSettings()
    {
        JsonSerializerSettings settings = ObjectExtensions.GetJSONSettings();
        settings.TypeNameHandling = TypeNameHandling.None;
        return settings;
    }
}