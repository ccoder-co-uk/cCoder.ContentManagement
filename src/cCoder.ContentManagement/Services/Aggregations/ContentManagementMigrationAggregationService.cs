// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.ComponentModel.DataAnnotations;
using cCoder.ContentManagement.Brokers;
using Newtonsoft.Json.Linq;
using cCoder.ContentManagement.Services.Orchestrations;
using cCoder.ContentManagement.Services.Processings;
using cCoder.ContentManagement.Models;
using cCoder.Data.Models.Packaging;
using cCoder.Data.Models.CMS;

namespace cCoder.ContentManagement.Services.Aggregations;

internal partial class ContentManagementMigrationAggregationService(
    IJsonBroker jsonBroker,
    IPackageExportProcessingService packageExportProcessingService,
    IComponentOrchestrationService componentOrchestrationService,
    ILayoutOrchestrationService layoutOrchestrationService,
    IPageOrchestrationService pageOrchestrationService,
    IPageRoleOrchestrationService pageRoleOrchestrationService,
    IResourceOrchestrationService resourceOrchestrationService,
    ITemplateOrchestrationService templateOrchestrationService,
    IScriptOrchestrationService scriptOrchestrationService)
        : IContentManagementMigrationAggregationService
{
    private static readonly HashSet<string> ComputedImportFields = new(comparer: StringComparer.OrdinalIgnoreCase)
    {
        "LastUpdated",
        "LastUpdatedBy",
        "CreatedOn",
        "CreatedBy"
    };

    public Package[] ExportPackages(int appId, string[] packageNames) =>
        TryCatch<Package[]>(operation: () =>
    {
        ValidateExportPackages(inputs: [appId, packageNames]);
        ValidateAppId(appId: appId, parameterName: "appId");

        return ValidatePackageNames(
            packageNames: packageNames,
            parameterName: "packageNames")
            .Select(selector: packageName =>
                packageExportProcessingService.ExportPackage(
                    appId: appId,
                    packageName: packageName))
            .ToArray();

    });

    public ValueTask ImportPackageAsync(int appId, Package package) =>
        TryCatch(operation: async () =>
    {
        ValidateImportPackageAsync(inputs: [appId, package]);
        ValidateAppId(appId: appId, parameterName: "appId");
        ValidatePackage(package: package, parameterName: "package");

        if (package.Items != null)
        {
            foreach (PackageItem item in package.Items)
            {
                switch (item.Type)
                {
                    case "Core/Component":
                        await ImportComponentsAsync(appId: appId, item: item);
                        break;
                    case "Core/Layout":
                        await ImportLayoutsAsync(appId: appId, item: item);
                        break;
                    case "Core/Page":
                        await ImportPagesAsync(appId: appId, item: item);
                        break;
                    case "Core/PageRole":
                        await ImportPageRolesAsync(appId: appId, item: item);
                        break;
                    case "Core/Resource":
                        await ImportResourcesAsync(appId: appId, item: item);
                        break;
                    case "Core/Script":
                        await ImportScriptsAsync(appId: appId, item: item);
                        break;
                    case "Core/Template":
                        await ImportTemplatesAsync(appId: appId, item: item);
                        break;
                }
            }
        }

    }, isValueTask: true);

    private async ValueTask ImportComponentsAsync(int appId, PackageItem item)
    {
        ValidateAppId(appId: appId, parameterName: "appId");
        ValidatePackageItem(packageItem: item, parameterName: "item");
        string sanitizedData = RemoveComputedFields(json: item.Data);
        Component[] items = ((!sanitizedData.StartsWith(value: "{")) ? jsonBroker.ParseJson<Component[]>(json: sanitizedData) : new Component[1] { jsonBroker.ParseJson<Component>(json: sanitizedData) });
        await componentOrchestrationService.ImportComponentsAsync(appId: appId, items: items);
    }

    private async ValueTask ImportLayoutsAsync(int appId, PackageItem item)
    {
        ValidateAppId(appId: appId, parameterName: "appId");
        ValidatePackageItem(packageItem: item, parameterName: "item");
        string sanitizedData = RemoveComputedFields(json: item.Data);
        Layout[] items = ((!sanitizedData.StartsWith(value: "{")) ? jsonBroker.ParseJson<Layout[]>(json: sanitizedData) : new Layout[1] { jsonBroker.ParseJson<Layout>(json: sanitizedData) });
        await layoutOrchestrationService.ImportLayoutsAsync(appId: appId, items: items);
    }

    private async ValueTask ImportPagesAsync(int appId, PackageItem item)
    {
        ValidateAppId(appId: appId, parameterName: "appId");
        ValidatePackageItem(packageItem: item, parameterName: "item");
        string sanitizedData = RemoveComputedFields(json: item.Data);
        Page[] pages = ((!sanitizedData.StartsWith(value: "{")) ? jsonBroker.ParseJson<Page[]>(json: sanitizedData) : new Page[1] { jsonBroker.ParseJson<Page>(json: sanitizedData) });
        await pageOrchestrationService.ImportPagesAsync(appId: appId, items: pages);
    }

    private async ValueTask ImportPageRolesAsync(int appId, PackageItem item)
    {
        ValidateAppId(appId: appId, parameterName: "appId");
        ValidatePackageItem(packageItem: item, parameterName: "item");
        PageRoleInfo[] pageRoles = ((!item.Data.StartsWith(value: "{")) ? jsonBroker.ParseJson<PageRoleInfo[]>(json: item.Data) : new PageRoleInfo[1] { jsonBroker.ParseJson<PageRoleInfo>(json: item.Data) });

        await pageRoleOrchestrationService.ImportPageRoleInfosAsync(
            appId: appId,
            items: pageRoles);
    }

    private async ValueTask ImportResourcesAsync(int appId, PackageItem item)
    {
        ValidateAppId(appId: appId, parameterName: "appId");
        ValidatePackageItem(packageItem: item, parameterName: "item");
        string sanitizedData = RemoveComputedFields(json: item.Data);
        Resource[] items = ((!sanitizedData.StartsWith(value: "{")) ? jsonBroker.ParseJson<Resource[]>(json: sanitizedData) : new Resource[1] { jsonBroker.ParseJson<Resource>(json: sanitizedData) });
        await resourceOrchestrationService.ImportResourcesAsync(appId: appId, items: items);
    }

    private async ValueTask ImportScriptsAsync(int appId, PackageItem item)
    {
        ValidateAppId(appId: appId, parameterName: "appId");
        ValidatePackageItem(packageItem: item, parameterName: "item");
        string sanitizedData = RemoveComputedFields(json: item.Data);
        Script[] items = ((!sanitizedData.StartsWith(value: "{")) ? jsonBroker.ParseJson<Script[]>(json: sanitizedData) : new Script[1] { jsonBroker.ParseJson<Script>(json: sanitizedData) });
        await scriptOrchestrationService.ImportScriptsAsync(appId: appId, items: items);
    }

    private async ValueTask ImportTemplatesAsync(int appId, PackageItem item)
    {
        ValidateAppId(appId: appId, parameterName: "appId");
        ValidatePackageItem(packageItem: item, parameterName: "item");
        string sanitizedData = RemoveComputedFields(json: item.Data);
        Template[] items = ((!sanitizedData.StartsWith(value: "{")) ? jsonBroker.ParseJson<Template[]>(json: sanitizedData) : new Template[1] { jsonBroker.ParseJson<Template>(json: sanitizedData) });
        await templateOrchestrationService.ImportTemplatesAsync(appId: appId, items: items);
    }

    private static string RemoveComputedFields(string json)
    {
        if (string.IsNullOrWhiteSpace(value: json))
        {
            return json;
        }

        JToken token = JToken.Parse(json: json);
        RemoveComputedFields(token: token);
        return token.ToString();
    }

    private static void RemoveComputedFields(JToken token)
    {
        if (token is JObject jsonObject)
        {
            JProperty[] computedProperties = jsonObject.Properties()
                .Where(predicate: property => ComputedImportFields.Contains(item: property.Name))
                .ToArray();

            foreach (JProperty property in computedProperties)
            {
                property.Remove();
            }

            foreach (JProperty property in jsonObject.Properties()
                .ToArray())
            {
                RemoveComputedFields(token: property.Value);
            }
        }
        else
        {
            if (token is JArray jsonArray)
            {
                foreach (JToken arrayItem in jsonArray)
                {
                    RemoveComputedFields(token: arrayItem);
                }
            }
        }
    }

    private static int ValidateAppId(int appId, string parameterName)
    {
        if (appId < 1)
        {
            throw new ValidationException(message: parameterName + " must be greater than 0.");
        }

        return appId;
    }

    private static Package ValidatePackage(Package package, string parameterName)
    {
        if (package == null)
        {
            throw new ValidationException(message: parameterName + " is required.");
        }

        return package;
    }

    private static string[] ValidatePackageNames(
        string[] packageNames,
        string parameterName)
    {
        if (packageNames == null)
        {
            throw new ValidationException(
                message: parameterName + " is required.");
        }

        return packageNames;
    }

    private static PackageItem ValidatePackageItem(PackageItem packageItem, string parameterName)
    {
        if (packageItem == null)
        {
            throw new ValidationException(message: parameterName + " is required.");
        }

        if (string.IsNullOrWhiteSpace(value: packageItem.Type))
        {
            throw new ValidationException(message: parameterName + ".Type is required.");
        }

        if (string.IsNullOrWhiteSpace(value: packageItem.Data))
        {
            throw new ValidationException(message: parameterName + ".Data is required.");
        }

        return packageItem;
    }
}