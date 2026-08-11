// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json.Linq;
using cCoder.ContentManagement.Services.Orchestrations;
using cCoder.ContentManagement.Models;
using cCoder.Data.Models.Packaging;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models;
using System.Text.Json;

namespace cCoder.ContentManagement.Services.Aggregations;

internal partial class ContentManagementMigrationAggregationService(
    IMigrationSupportOrchestrationService migrationSupportOrchestrationService,
    IComponentOrchestrationService componentOrchestrationService,
    ILayoutOrchestrationService layoutOrchestrationService,
    IPageOrchestrationService pageOrchestrationService,
    IPageImportOrchestrationService pageImportOrchestrationService,
    IResourceOrchestrationService resourceOrchestrationService,
    ITemplateOrchestrationService templateOrchestrationService,
    IScriptOrchestrationService scriptOrchestrationService,
    ICommonObjectOrchestrationService commonObjectOrchestrationService,
    PageRenderCacheImportState pageRenderCacheImportState)
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

        return migrationSupportOrchestrationService.ExportPackages(
            appId: appId,
            packageNames: ValidatePackageNames(
                packageNames: packageNames,
                parameterName: "packageNames"));

    });

    public ValueTask ImportPackageAsync(int? appId, Package package) =>
        TryCatch(operation: async () =>
    {
        ValidateImportPackageAsync(inputs: [appId, package]);
        ValidatePackage(package: package, parameterName: "package");

        if (appId is null)
        {
            await ImportCommonCachePackageAsync(package: package);
            return;
        }

        ValidateAppId(appId: appId.Value, parameterName: "appId");

        pageRenderCacheImportState.Active = true;

        try
        {
            if (package.Items != null)
            {
                foreach (PackageItem item in package.Items)
                {
                    await ImportPackageItemAsync(appId: appId.Value, item: item);
                }
            }
        }
        finally
        {
            pageRenderCacheImportState.Active = false;
        }

    }, isValueTask: true);

    private async ValueTask ImportCommonCachePackageAsync(Package package)
    {
        CommonObject[] commonObjects =
        [
            .. package.Items?
                .SelectMany(selector: item =>
                    ConvertToCommonObjects(
                        package: package,
                        item: item)) ?? []
        ];

        if (commonObjects.Length > 0)
        {
            await commonObjectOrchestrationService
                .AddAllCommonObjectsAsync(
                    newCommonObjects: commonObjects);
        }

    }

    private static IEnumerable<CommonObject> ConvertToCommonObjects(
        Package package,
        PackageItem item)
    {
        using JsonDocument document = JsonDocument.Parse(json: item.Data);

        IEnumerable<JsonElement> records =
            document.RootElement.ValueKind == JsonValueKind.Array
                ? document.RootElement.EnumerateArray()
                : [document.RootElement];

        foreach (JsonElement record in records)
        {
            yield return new CommonObject
            {
                Name = ReadRequiredString(record: record, name: "Name"),
                Description = ReadOptionalString(record: record, name: "Description"),
                Version = 1,
                Key = ReadOptionalString(record: record, name: "ResourceKey")
                    ?? ReadOptionalString(record: record, name: "Key")
                    ?? package.Category,
                Type = item.Type,
                Json = record.GetRawText(),
                Culture = ReadOptionalString(record: record, name: "Culture")
                    ?? string.Empty,
                CreatedOn = ReadOptionalDateTimeOffset(
                    record: record,
                    name: "CreatedOn"),
                LastUpdated = ReadOptionalDateTimeOffset(
                    record: record,
                    name: "LastUpdated")
            };
        }
    }

    private static DateTimeOffset ReadOptionalDateTimeOffset(
        JsonElement record,
        string name) =>
        record.TryGetProperty(propertyName: name, value: out JsonElement value)
            && value.TryGetDateTimeOffset(value: out DateTimeOffset result)
                ? result
                : DateTimeOffset.UtcNow;

    private static string ReadOptionalString(
        JsonElement record,
        string name) =>
        record.TryGetProperty(propertyName: name, value: out JsonElement value)
            && value.ValueKind == JsonValueKind.String
                ? value.GetString()
                : null;

    private static string ReadRequiredString(
        JsonElement record,
        string name) =>
        ReadOptionalString(record: record, name: name)
            ?? throw new ValidationException(
                message: name + " is required.");

    private async ValueTask ImportPackageItemAsync(int appId, PackageItem item)
    {
        switch (item.Type)
        {
            case "ContentManagement/Component":
                await ImportComponentsAsync(appId: appId, item: item);
                break;
            case "ContentManagement/Layout":
                await ImportLayoutsAsync(appId: appId, item: item);
                break;
            case "ContentManagement/Page":
                await ImportPagesAsync(appId: appId, item: item);
                break;
            case "ContentManagement/Resource":
                await ImportResourcesAsync(appId: appId, item: item);
                break;
            case "ContentManagement/Script":
                await ImportScriptsAsync(appId: appId, item: item);
                break;
            case "ContentManagement/Template":
                await ImportTemplatesAsync(appId: appId, item: item);
                break;
        }
    }

    private async ValueTask ImportComponentsAsync(int appId, PackageItem item)
    {
        ValidateAppId(appId: appId, parameterName: "appId");
        ValidatePackageItem(packageItem: item, parameterName: "item");
        string sanitizedData = RemoveComputedFields(json: item.Data);

        Component[] items = migrationSupportOrchestrationService
            .DeserializeItems<Component>(json: sanitizedData);

        await componentOrchestrationService.ImportComponentsAsync(appId: appId, items: items);
    }

    private async ValueTask ImportLayoutsAsync(int appId, PackageItem item)
    {
        ValidateAppId(appId: appId, parameterName: "appId");
        ValidatePackageItem(packageItem: item, parameterName: "item");
        string sanitizedData = RemoveComputedFields(json: item.Data);

        Layout[] items = migrationSupportOrchestrationService
            .DeserializeItems<Layout>(json: sanitizedData);

        await layoutOrchestrationService.ImportLayoutsAsync(appId: appId, items: items);
    }

    private async ValueTask ImportPagesAsync(int appId, PackageItem item)
    {
        ValidateAppId(appId: appId, parameterName: "appId");
        ValidatePackageItem(packageItem: item, parameterName: "item");
        string sanitizedData = RemoveComputedFields(json: item.Data);

        Page[] pages = migrationSupportOrchestrationService
            .DeserializeItems<Page>(json: sanitizedData);

        Page[] importedPages = await pageOrchestrationService.ImportPagesAsync(
            appId: appId,
            items: pages);

        foreach (Page page in importedPages)
        {
            await pageImportOrchestrationService.HandlePageImportAsync(page: page);
        }
    }

    private async ValueTask ImportResourcesAsync(int appId, PackageItem item)
    {
        ValidateAppId(appId: appId, parameterName: "appId");
        ValidatePackageItem(packageItem: item, parameterName: "item");
        string sanitizedData = RemoveComputedFields(json: item.Data);

        Resource[] items = migrationSupportOrchestrationService
            .DeserializeItems<Resource>(json: sanitizedData);

        await resourceOrchestrationService.ImportResourcesAsync(appId: appId, items: items);
    }

    private async ValueTask ImportScriptsAsync(int appId, PackageItem item)
    {
        ValidateAppId(appId: appId, parameterName: "appId");
        ValidatePackageItem(packageItem: item, parameterName: "item");
        string sanitizedData = RemoveComputedFields(json: item.Data);

        Script[] items = migrationSupportOrchestrationService
            .DeserializeItems<Script>(json: sanitizedData);

        await scriptOrchestrationService.ImportScriptsAsync(appId: appId, items: items);
    }

    private async ValueTask ImportTemplatesAsync(int appId, PackageItem item)
    {
        ValidateAppId(appId: appId, parameterName: "appId");
        ValidatePackageItem(packageItem: item, parameterName: "item");
        string sanitizedData = RemoveComputedFields(json: item.Data);

        Template[] items = migrationSupportOrchestrationService
            .DeserializeItems<Template>(json: sanitizedData);

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