using System.Security;
using cCoder.ContentManagement.Brokers;
using cCoder.ContentManagement.Brokers.Storages;
using cCoder.Data.Extensions;
using Newtonsoft.Json;
using Package = cCoder.Data.Models.Packaging.Package;
using PackageItem = cCoder.Data.Models.Packaging.PackageItem;

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
        EnsureAdmin(ValidateAppId(appId, "appId"));

        return CreatePackage(
            "Roles",
            "Core/Role",
            roleBroker.GetAllRoles(ignoreFilters: true)
                .Where(role => role.AppId == appId)
                .Select(role => new { role.Name, role.Privs })
                .ToArray());
    }

    public Package ExportLayouts(int appId)
    {
        EnsureAdmin(ValidateAppId(appId, "appId"));

        return CreatePackage(
            "Layouts",
            "Core/Layout",
            layoutBroker.GetAllLayouts(ignoreFilters: true)
                .Where(layout => layout.AppId == appId)
                .Select(layout => new
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
        EnsureAdmin(ValidateAppId(appId, "appId"));

        return CreatePackage(
            "Templates",
            "Core/Template",
            templateBroker.GetAllTemplates(ignoreFilters: true)
                .Where(template => template.AppId == appId)
                .Select(template => new
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
        EnsureAdmin(ValidateAppId(appId, "appId"));

        return CreatePackage(
            "Components",
            "Core/Component",
            componentBroker.GetAllComponents(ignoreFilters: true)
                .Where(component => component.AppId == appId)
                .Select(component => new
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
        EnsureAdmin(ValidateAppId(appId, "appId"));

        return CreatePackage(
            "Scripts",
            "Core/Script",
            scriptBroker.GetAllScripts(ignoreFilters: true)
                .Where(script => script.AppId == appId)
                .Select(script => new
                {
                    script.Name,
                    script.Content,
                    script.LastUpdated
                })
                .ToArray());
    }

    public Package ExportResources(int appId)
    {
        EnsureAdmin(ValidateAppId(appId, "appId"));

        return CreatePackage(
            "Resources",
            "Core/Resource",
            resourceBroker.GetAllResources(ignoreFilters: true)
                .Where(resource => resource.AppId == appId)
                .Select(resource => new
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
        EnsureAdmin(ValidateAppId(appId, "appId"));

        List<ExportPage> pages = pageBroker.GetAllPages(ignoreFilters: true)
            .Where(page => page.AppId == appId)
            .Select(page => new ExportPage
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
                    .Select(content => new ExportContent
                    {
                        CultureId = content.CultureId,
                        Name = content.Name,
                        Html = content.Html
                    })
                    .ToArray(),
                PageInfo = page.PageInfo
                    .Select(info => new ExportPageInfo
                    {
                        CultureId = info.CultureId,
                        Description = info.Description,
                        Keywords = info.Keywords,
                        Title = info.Title
                    })
                    .ToArray()
            })
            .ToList();

        Dictionary<int, ExportPage> pagesById = pages.ToDictionary(page => page.Id);

        foreach (ExportPage page in pages.Where(page => page.ParentId.HasValue))
        {
            ExportPage root = page;

            while (root.ParentId.HasValue && pagesById.TryGetValue(root.ParentId.Value, out ExportPage parent))
            {
                root = parent;
            }

            if (string.IsNullOrEmpty(root.Path) && !string.IsNullOrEmpty(page.Path))
            {
                page.Path = "/" + page.Path.TrimStart('/');
            }
        }

        return CreatePackage(
            "Pages",
            "Core/Page",
            pages.Select(page => new
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
            }).ToArray());
    }

    public Package ExportPageRoles(int appId)
    {
        EnsureAdmin(ValidateAppId(appId, "appId"));

        return CreatePackage(
            "PageRoles",
            "Core/PageRole",
            pageBroker.GetAllPages(ignoreFilters: true)
                .Where(page => page.AppId == appId)
                .SelectMany(page => page.Roles.Select(role => new
                {
                    page.Path,
                    Role = role.Role.Name
                }))
                .ToArray());
    }

    private Package CreatePackage(string name, string itemType, object data)
    {
        return new Package(name)
        {
            Items =
            [
                new PackageItem
                {
                    Type = itemType,
                    Data = JsonConvert.SerializeObject(data, CreateSerializerSettings())
                }
            ]
        };
    }

    private void EnsureAdmin(int appId)
    {
        if (!authorizationBroker.IsAdminOfApp(appId))
        {
            throw new SecurityException("Access Denied!");
        }
    }

    private static JsonSerializerSettings CreateSerializerSettings()
    {
        JsonSerializerSettings settings = ObjectExtensions.GetJSONSettings();
        settings.TypeNameHandling = TypeNameHandling.None;
        return settings;
    }
}

