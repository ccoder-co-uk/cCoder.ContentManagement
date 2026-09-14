// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Models;
using cCoder.ContentManagement.Services.Processings;
using cCoder.Data.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Packaging;

namespace cCoder.ContentManagement.Services.Orchestrations;

internal sealed partial class ContentManagementPackageOrchestrationService(
    IJsonProcessingService jsonProcessingService,
    IPackageImportEventProcessingService packageImportEventProcessingService,
    IPackageExportProcessingService packageExportProcessingService,
    IAuthorizationProcessingService authorizationProcessingService)
    : IContentManagementPackageOrchestrationService
{
    private static readonly HashSet<string> ComputedImportFields =
        new(comparer: StringComparer.OrdinalIgnoreCase)
        {
            "LastUpdated",
            "LastUpdatedBy",
            "CreatedOn",
            "CreatedBy"
        };

    public ValueTask ImportPackageAsync(int? appId, Package package) =>
        TryCatch(operation: async () =>
    {
        ValidateImportPackageAsync(inputs: [appId, package]);

        if (appId is null)
        {
            await RaiseCommonObjectImportAsync(package: package);
            return;
        }

        foreach (PackageItem item in package.Items ?? [])
        {
            await RaiseAppImportAsync(appId: appId.Value, item: item);
        }
    }, isValueTask: true);

    public Package ExportPackage(int appId, string packageName) =>
        TryCatch<Package>(operation: () =>
    {
        ValidateExportPackage(inputs: [appId, packageName]);

        return packageExportProcessingService.ExportPackage(
            appId: appId,
            packageName: packageName);
    });

    private async ValueTask RaiseAppImportAsync(int appId, PackageItem item)
    {
        ArgumentNullException.ThrowIfNull(argument: item);

        string sanitizedData = jsonProcessingService.RemovePropertiesRecursively(
            json: item.Data,
            propertyNames: ComputedImportFields);

        if (string.Equals(
            a: item.Type,
            b: "ContentManagement/Component",
            comparisonType: StringComparison.OrdinalIgnoreCase))
        {
            await RaiseAsync(
                eventName: "component_import",
                appId: appId,
                items: jsonProcessingService.DeserializeItems<Component>(json: sanitizedData));
        }
        else if (string.Equals(
            a: item.Type,
            b: "ContentManagement/Layout",
            comparisonType: StringComparison.OrdinalIgnoreCase))
        {
            await RaiseAsync(
                eventName: "layout_import",
                appId: appId,
                items: jsonProcessingService.DeserializeItems<Layout>(json: sanitizedData));
        }
        else if (string.Equals(
            a: item.Type,
            b: "ContentManagement/Page",
            comparisonType: StringComparison.OrdinalIgnoreCase))
        {
            await RaiseAsync(
                eventName: "page_import",
                appId: appId,
                items: jsonProcessingService.DeserializeItems<Page>(json: sanitizedData));
        }
        else if (string.Equals(
            a: item.Type,
            b: "ContentManagement/Resource",
            comparisonType: StringComparison.OrdinalIgnoreCase))
        {
            await RaiseAsync(
                eventName: "resource_import",
                appId: appId,
                items: jsonProcessingService.DeserializeItems<Resource>(json: sanitizedData));
        }
        else if (string.Equals(
            a: item.Type,
            b: "ContentManagement/Script",
            comparisonType: StringComparison.OrdinalIgnoreCase))
        {
            await RaiseAsync(
                eventName: "script_import",
                appId: appId,
                items: jsonProcessingService.DeserializeItems<Script>(json: sanitizedData));
        }
        else if (string.Equals(
            a: item.Type,
            b: "ContentManagement/Template",
            comparisonType: StringComparison.OrdinalIgnoreCase))
        {
            await RaiseAsync(
                eventName: "template_import",
                appId: appId,
                items: jsonProcessingService.DeserializeItems<Template>(json: sanitizedData));
        }
    }

    private ValueTask RaiseAsync<T>(string eventName, int appId, T[] items) =>
        packageImportEventProcessingService.RaiseImportAsync(
            eventName: eventName,
            import: new PackageItemImportEvent<T>
            {
                AppId = appId,
                Items = items
            },
            userId: authorizationProcessingService.GetCurrentUserId());

    private async ValueTask RaiseCommonObjectImportAsync(Package package)
    {
        CommonObject[] commonObjects =
        [
            .. package.Items?
                .SelectMany(selector: item => ConvertToCommonObjects(
                    package: package,
                    item: item)) ?? []
        ];

        if (commonObjects.Length > 0)
        {
            await packageImportEventProcessingService.RaiseImportAsync(
                eventName: "common_objects_import",
                import: new PackageItemImportEvent<CommonObject>
                {
                    AppId = null,
                    Items = commonObjects
                },
                userId: authorizationProcessingService.GetCurrentUserId());

        }
    }

    private IEnumerable<CommonObject> ConvertToCommonObjects(
        Package package,
        PackageItem item)
    {
        if (item == null || string.IsNullOrWhiteSpace(value: item.Data))
        {
            yield break;
        }

        Dictionary<string, object>[] records =
            jsonProcessingService.DeserializeItems<Dictionary<string, object>>(
                json: item.Data);

        foreach (Dictionary<string, object> record in records)
        {
            string key = GetString(record: record, name: "ResourceKey")
                ?? GetString(record: record, name: "Key")
                ?? package.Category;

            yield return new CommonObject
            {
                Name = GetString(record: record, name: "Name"),
                Description = GetString(record: record, name: "Description"),
                Version = 1,
                Key = key,
                Type = item.Type,
                Json = jsonProcessingService.Serialize(value: record),
                Culture = GetString(record: record, name: "Culture"),
                CreatedOn = GetDateTimeOffset(record: record, name: "CreatedOn"),
                LastUpdated = GetDateTimeOffset(record: record, name: "LastUpdated")
            };
        }
    }

    private static DateTimeOffset GetDateTimeOffset(
        IReadOnlyDictionary<string, object> record,
        string name)
    {
        object value = GetValue(record: record, name: name);

        return value switch
        {
            DateTimeOffset dateTimeOffset => dateTimeOffset,
            DateTime dateTime => new DateTimeOffset(dateTime: dateTime),
            _ when DateTimeOffset.TryParse(
                input: value?.ToString(),
                result: out DateTimeOffset parsed) => parsed,
            _ => DateTimeOffset.UtcNow
        };
    }

    private static string GetString(
        IReadOnlyDictionary<string, object> record,
        string name) =>
        GetValue(record: record, name: name)?.ToString();

    private static object GetValue(
        IReadOnlyDictionary<string, object> record,
        string name) =>
        record.FirstOrDefault(predicate: property => string.Equals(
            a: property.Key,
            b: name,
            comparisonType: StringComparison.OrdinalIgnoreCase)).Value;
}