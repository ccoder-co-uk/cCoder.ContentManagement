// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Text.Json;
using cCoder.Data;
using cCoder.Data.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Packaging;
using Newtonsoft.Json;

namespace Web.AcceptanceTests.Infrastructure;

internal static class AcceptanceSeedData
{
    public static Package[] LoadExportPackages()
    {
        using JsonDocument json = AcceptanceAssetLoader.LoadJson(fileName: "App.1.Export.json");
        JsonElement value = json.RootElement.GetProperty(propertyName: "value");

        Package[] packages = JsonConvert.DeserializeObject<Package[]>(
value: value.GetRawText(),
settings: cCoder.Data.Extensions.ObjectExtensions.GetJSONSettings());

        foreach (PackageItem item in packages.SelectMany(selector: package => package.Items))
        {
            item.Type = NormalizeContentManagementType(type: item.Type);
        }

        return packages;
    }

    public static Layout[] LoadLayoutPackageItems(string packageName, string itemType) =>
        LoadPackageItems(packageName: packageName, itemType: itemType, itemTypeContract: typeof(Layout))
        .Cast<Layout>()
        .ToArray();

    public static Template[] LoadTemplatePackageItems(string packageName, string itemType) =>
        LoadPackageItems(packageName: packageName, itemType: itemType, itemTypeContract: typeof(Template))
        .Cast<Template>()
        .ToArray();

    public static Resource[] LoadResourcePackageItems(string packageName, string itemType) =>
        LoadPackageItems(packageName: packageName, itemType: itemType, itemTypeContract: typeof(Resource))
        .Cast<Resource>()
        .ToArray();

    public static Component[] LoadComponentPackageItems(string packageName, string itemType) =>
        LoadPackageItems(packageName: packageName, itemType: itemType, itemTypeContract: typeof(Component))
        .Cast<Component>()
        .ToArray();

    public static Script[] LoadScriptPackageItems(string packageName, string itemType) =>
        LoadPackageItems(packageName: packageName, itemType: itemType, itemTypeContract: typeof(Script))
        .Cast<Script>()
        .ToArray();

    private static object[] LoadPackageItems(
        string packageName,
        string itemType,
        Type itemTypeContract)
    {
        Package package = LoadExportPackages()
            .First(predicate: found =>
            string.Equals(a: found.Name, b: packageName, comparisonType: StringComparison.OrdinalIgnoreCase)
        );

        return package.Items
            .Where(predicate: item => string.Equals(a: item.Type, b: itemType, comparisonType: StringComparison.OrdinalIgnoreCase))
            .SelectMany(selector: item =>
                UnpackItems(data: item.Data, itemTypeContract: itemTypeContract))
            .ToArray();
    }

    public static CommonObject[] LoadCommonObjects()
    {
        List<CommonObject> result = [];

        result.AddRange(collection: LoadCommonObjects(fileName: "Core.Resource.latest.json"));
        result.AddRange(collection: LoadCommonObjects(fileName: "Core.Component.latest.json"));
        result.AddRange(collection: LoadCommonObjects(fileName: "Core.Script.latest.json"));

        return result.ToArray();
    }

    private static CommonObject[] LoadCommonObjects(string fileName)
    {
        using JsonDocument json = AcceptanceAssetLoader.LoadJson(fileName: fileName);

        JsonElement value =
            json.RootElement.ValueKind == JsonValueKind.Object
                ? json.RootElement.GetProperty(propertyName: "value")
                : json.RootElement;

        CommonObject[] commonObjects = JsonConvert.DeserializeObject<CommonObject[]>(
value: value.GetRawText(),
settings: cCoder.Data.Extensions.ObjectExtensions.GetJSONSettings());

        foreach (CommonObject commonObject in commonObjects)
        {
            commonObject.Type = NormalizeContentManagementType(type: commonObject.Type);
        }

        return commonObjects;
    }

    private static string NormalizeContentManagementType(string type) =>
        type is "Core/Component"
            or "Core/Layout"
            or "Core/Page"
            or "Core/PageRole"
            or "Core/Resource"
            or "Core/Script"
            or "Core/Template"
                ? $"ContentManagement/{type["Core/".Length..]}"
                : type;

    private static IEnumerable<object> UnpackItems(string data, Type itemTypeContract)
    {
        string trimmed = data.TrimStart();

        if (trimmed.StartsWith(value: "[", comparisonType: StringComparison.Ordinal))
        {
            Array items = (Array)JsonConvert.DeserializeObject(
                value: trimmed,
                type: itemTypeContract.MakeArrayType(),
                settings: cCoder.Data.Extensions.ObjectExtensions.GetJSONSettings());

            return items.Cast<object>();
        }

        object item = JsonConvert.DeserializeObject(
            value: trimmed,
            type: itemTypeContract,
            settings: cCoder.Data.Extensions.ObjectExtensions.GetJSONSettings());

        return [item];
    }
}