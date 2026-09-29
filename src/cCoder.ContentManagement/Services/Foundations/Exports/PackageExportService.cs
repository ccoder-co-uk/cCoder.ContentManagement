// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using cCoder.ContentManagement.Brokers;
using cCoder.ContentManagement.Brokers.Exports;
using cCoder.ContentManagement.Models.Exports;
using cCoder.Data.Models.CMS;
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
data: packageExportBroker.Materialize(query: packageExportBroker.GetRoles()
            .Where(predicate: role => role.AppId == appId)
            .Select(selector: role => new { role.Name, role.Privs })));

    });

    public Package ExportLayoutsPackage(int appId) =>
        TryCatch<Package>(operation: () =>
    {
        ValidateExportLayoutsPackage(inputs: [appId]);
        ValidateAppId(appId: appId, parameterName: "appId");

        return CreatePackage(
name: "Layouts",
itemType: "ContentManagement/Layout",
data: packageExportBroker.Materialize(query: packageExportBroker.GetLayouts()
            .Where(predicate: layout => layout.AppId == appId)
            .Select(selector: layout => new
            {
                layout.Name,
                layout.HeaderHtml,
                layout.Html,
                layout.Script,
                layout.LastUpdated
            })));

    });

    public Package ExportTemplatesPackage(int appId) =>
        TryCatch<Package>(operation: () =>
    {
        ValidateExportTemplatesPackage(inputs: [appId]);
        ValidateAppId(appId: appId, parameterName: "appId");

        return CreatePackage(
name: "Templates",
itemType: "ContentManagement/Template",
data: packageExportBroker.Materialize(query: packageExportBroker.GetTemplates()
            .Where(predicate: template => template.AppId == appId)
            .Select(selector: template => new
            {
                template.Name,
                template.ResourceKey,
                template.RawString,
                template.LastUpdated
            })));

    });

    public Package ExportComponentsPackage(int appId) =>
        TryCatch<Package>(operation: () =>
    {
        ValidateExportComponentsPackage(inputs: [appId]);
        ValidateAppId(appId: appId, parameterName: "appId");

        return CreatePackage(
name: "Components",
itemType: "ContentManagement/Component",
data: packageExportBroker.Materialize(query: packageExportBroker.GetComponents()
            .Where(predicate: component => component.AppId == appId)
            .Select(selector: component => new
            {
                component.Name,
                component.Key,
                component.ResourceKey,
                component.Script,
                component.Content,
                component.LastUpdated
            })));

    });

    public Package ExportScriptsPackage(int appId) =>
        TryCatch<Package>(operation: () =>
    {
        ValidateExportScriptsPackage(inputs: [appId]);
        ValidateAppId(appId: appId, parameterName: "appId");

        return CreatePackage(
name: "Scripts",
itemType: "ContentManagement/Script",
data: packageExportBroker.Materialize(query: packageExportBroker.GetScripts()
            .Where(predicate: script => script.AppId == appId)
            .Select(selector: script => new
            {
                script.Name,
                script.Description,
                script.Key,
                script.Content,
                script.LastUpdated
            })));

    });

    public Package ExportResourcesPackage(int appId) =>
        TryCatch<Package>(operation: () =>
    {
        ValidateExportResourcesPackage(inputs: [appId]);
        ValidateAppId(appId: appId, parameterName: "appId");

        return CreatePackage(
name: "Resources",
itemType: "ContentManagement/Resource",
data: packageExportBroker.Materialize(query: packageExportBroker.GetResources()
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
            })));

    });

    public Package ExportPagesPackage(int appId) =>
        TryCatch<Package>(operation: () =>
    {
        ValidateExportPagesPackage(inputs: [appId]);
        ValidateAppId(appId: appId, parameterName: "appId");

        List<ExportPage> pages = [];

        foreach (Page page in packageExportBroker.GetPagesWithContent()
            .Where(predicate: page => page.AppId == appId))
        {
            List<ExportContent> contents = [];

            foreach (Content content in page.Contents)
            {
                contents.Add(item: new ExportContent
                {
                    CultureId = content.CultureId,
                    Name = content.Name,
                    Html = content.Html
                });
            }

            List<ExportPageInfo> pageInfo = [];

            foreach (PageInfo info in page.PageInfo)
            {
                pageInfo.Add(item: new ExportPageInfo
                {
                    CultureId = info.CultureId,
                    Description = info.Description,
                    Keywords = info.Keywords,
                    Title = info.Title
                });
            }

            pages.Add(item: new ExportPage
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
                Contents = [.. contents],
                PageInfo = [.. pageInfo]
            });
        }

        Dictionary<int, ExportPage> pagesById = [];

        foreach (ExportPage page in pages)
        {
            pagesById[page.Id] = page;
        }

        foreach (ExportPage page in pages)
        {
            if (!page.ParentId.HasValue)
            {
                continue;
            }

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

        List<object> packagePages = [];

        foreach (ExportPage page in pages)
        {
            packagePages.Add(item: new
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
            });
        }

        return CreatePackage(
name: "Pages",
itemType: "ContentManagement/Page",
data: packagePages);

    });

    public Package ExportPageRolesPackage(int appId) =>
        TryCatch<Package>(operation: () =>
    {
        ValidateExportPageRolesPackage(inputs: [appId]);
        ValidateAppId(appId: appId, parameterName: "appId");

        return CreatePackage(
name: "PageRoles",
itemType: "ContentManagement/PageRole",
data: packageExportBroker.Materialize(query: packageExportBroker.GetPages()
            .Where(predicate: page => page.AppId == appId)
            .SelectMany(
                collectionSelector: page => page.Roles,
                resultSelector: (page, role) => new
            {
                page.Path,
                Role = role.Role.Name
            })));

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