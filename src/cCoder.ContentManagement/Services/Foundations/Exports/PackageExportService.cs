// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Brokers;
using cCoder.ContentManagement.Brokers.Exports;
using cCoder.ContentManagement.Models.Exports;
using cCoder.Data.Models.Packaging;

namespace cCoder.ContentManagement.Services.Foundations.Exports;

internal partial class PackageExportService(
    IPackageExportBroker packageExportBroker,
    IJsonBroker jsonBroker) : IPackageExportService
{
    public Package ExportRolesPackage(int appId) =>
        TryCatch<Package>(operation: () =>
    {
        ValidateExportRolesPackage(inputs: [appId]);
        ValidateAppId(appId: appId, parameterName: "appId");

        return CreatePackage(
name: "Roles",
itemType: "Core/Role",
data: packageExportBroker.GetRoles()
            .Where(predicate: role => role.AppId == appId)
            .Select(selector: role => new { role.Name, role.Privs })
            .ToArray());

    });

    public Package ExportLayoutsPackage(int appId) =>
        TryCatch<Package>(operation: () =>
    {
        ValidateExportLayoutsPackage(inputs: [appId]);
        ValidateAppId(appId: appId, parameterName: "appId");

        return CreatePackage(
name: "Layouts",
itemType: "ContentManagement/Layout",
data: packageExportBroker.GetLayouts()
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

    });

    public Package ExportTemplatesPackage(int appId) =>
        TryCatch<Package>(operation: () =>
    {
        ValidateExportTemplatesPackage(inputs: [appId]);
        ValidateAppId(appId: appId, parameterName: "appId");

        return CreatePackage(
name: "Templates",
itemType: "ContentManagement/Template",
data: packageExportBroker.GetTemplates()
            .Where(predicate: template => template.AppId == appId)
            .Select(selector: template => new
            {
                template.Name,
                template.ResourceKey,
                template.RawString,
                template.LastUpdated
            })
            .ToArray());

    });

    public Package ExportComponentsPackage(int appId) =>
        TryCatch<Package>(operation: () =>
    {
        ValidateExportComponentsPackage(inputs: [appId]);
        ValidateAppId(appId: appId, parameterName: "appId");

        return CreatePackage(
name: "Components",
itemType: "ContentManagement/Component",
data: packageExportBroker.GetComponents()
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

    });

    public Package ExportScriptsPackage(int appId) =>
        TryCatch<Package>(operation: () =>
    {
        ValidateExportScriptsPackage(inputs: [appId]);
        ValidateAppId(appId: appId, parameterName: "appId");

        return CreatePackage(
name: "Scripts",
itemType: "ContentManagement/Script",
data: packageExportBroker.GetScripts()
            .Where(predicate: script => script.AppId == appId)
            .Select(selector: script => new
            {
                script.Name,
                script.Description,
                script.Key,
                script.Content,
                script.LastUpdated
            })
            .ToArray());

    });

    public Package ExportResourcesPackage(int appId) =>
        TryCatch<Package>(operation: () =>
    {
        ValidateExportResourcesPackage(inputs: [appId]);
        ValidateAppId(appId: appId, parameterName: "appId");

        return CreatePackage(
name: "Resources",
itemType: "ContentManagement/Resource",
data: packageExportBroker.GetResources()
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

    });

    public Package ExportPagesPackage(int appId) =>
        TryCatch<Package>(operation: () =>
    {
        ValidateExportPagesPackage(inputs: [appId]);
        ValidateAppId(appId: appId, parameterName: "appId");

        List<ExportPage> pages = packageExportBroker.GetPages()
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
itemType: "ContentManagement/Page",
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

    });

    public Package ExportPageRolesPackage(int appId) =>
        TryCatch<Package>(operation: () =>
    {
        ValidateExportPageRolesPackage(inputs: [appId]);
        ValidateAppId(appId: appId, parameterName: "appId");

        return CreatePackage(
name: "PageRoles",
itemType: "ContentManagement/PageRole",
data: packageExportBroker.GetPages()
            .Where(predicate: page => page.AppId == appId)
            .SelectMany(selector: page => page.Roles.Select(selector: role => new
            {
                page.Path,
                Role = role.Role.Name
            }))
            .ToArray());

    });

    private Package CreatePackage(string name, string itemType, object data) =>
        new Package
        {
            Name = name,
            Items =
            [
                new PackageItem
                {
                    Type = itemType,
                    Data = jsonBroker.SerializeIgnoringReferences(
                        value: data)
                }
            ]
        };

}