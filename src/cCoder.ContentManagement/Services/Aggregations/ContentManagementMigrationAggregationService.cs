using System.ComponentModel.DataAnnotations;
using cCoder.ContentManagement.Brokers;
using Newtonsoft.Json.Linq;
using cCoder.ContentManagement.Services.Orchestrations;
using Package = cCoder.Data.Models.Packaging.Package;
using PackageItem = cCoder.Data.Models.Packaging.PackageItem;
using PageRoleInfo = cCoder.ContentManagement.Models.PageRoleInfo;

namespace cCoder.ContentManagement.Services.Aggregations;

internal class ContentManagementMigrationAggregationService(
    IJsonBroker jsonBroker,
    IComponentOrchestrationService componentOrchestrationService,
    ILayoutOrchestrationService layoutOrchestrationService,
    IPageOrchestrationService pageOrchestrationService,
    IPageRoleOrchestrationService pageRoleOrchestrationService,
    IResourceOrchestrationService resourceOrchestrationService,
    ITemplateOrchestrationService templateOrchestrationService,
    IScriptOrchestrationService scriptOrchestrationService)
        : IContentManagementMigrationAggregationService
{
    private static readonly HashSet<string> ComputedImportFields = new(StringComparer.OrdinalIgnoreCase)
    {
        "LastUpdated",
        "LastUpdatedBy",
        "CreatedOn",
        "CreatedBy"
    };

    public async ValueTask ImportPackageAsync(int appId, Package package)
    {
        ValidateAppId(appId, "appId");
        ValidatePackage(package, "package");

        foreach (PackageItem item in package.Items ?? [])
        {
            switch (item.Type)
            {
                case "Core/Component":
                    await ImportComponentsAsync(appId, item);
                    break;
                case "Core/Layout":
                    await ImportLayoutsAsync(appId, item);
                    break;
                case "Core/Page":
                    await ImportPagesAsync(appId, item);
                    break;
                case "Core/PageRole":
                    await ImportPageRolesAsync(appId, item);
                    break;
                case "Core/Resource":
                    await ImportResourcesAsync(appId, item);
                    break;
                case "Core/Script":
                    await ImportScriptsAsync(appId, item);
                    break;
                case "Core/Template":
                    await ImportTemplatesAsync(appId, item);
                    break;
            }
        }
    }

    private async ValueTask ImportComponentsAsync(int appId, PackageItem item)
    {
        ValidateAppId(appId, "appId");
        ValidatePackageItem(item, "item");
        string sanitizedData = RemoveComputedFields(item.Data);
        cCoder.Data.Models.CMS.Component[] items = ((!sanitizedData.StartsWith("{")) ? jsonBroker.ParseJson<cCoder.Data.Models.CMS.Component[]>(sanitizedData) : new cCoder.Data.Models.CMS.Component[1] { jsonBroker.ParseJson<cCoder.Data.Models.CMS.Component>(sanitizedData) });
        await componentOrchestrationService.ImportComponentsAsync(appId, items);
    }

    private async ValueTask ImportLayoutsAsync(int appId, PackageItem item)
    {
        ValidateAppId(appId, "appId");
        ValidatePackageItem(item, "item");
        string sanitizedData = RemoveComputedFields(item.Data);
        cCoder.Data.Models.CMS.Layout[] items = ((!sanitizedData.StartsWith("{")) ? jsonBroker.ParseJson<cCoder.Data.Models.CMS.Layout[]>(sanitizedData) : new cCoder.Data.Models.CMS.Layout[1] { jsonBroker.ParseJson<cCoder.Data.Models.CMS.Layout>(sanitizedData) });
        await layoutOrchestrationService.ImportLayoutsAsync(appId, items);
    }

    private async ValueTask ImportPagesAsync(int appId, PackageItem item)
    {
        ValidateAppId(appId, "appId");
        ValidatePackageItem(item, "item");
        string sanitizedData = RemoveComputedFields(item.Data);
        cCoder.Data.Models.CMS.Page[] pages = ((!sanitizedData.StartsWith("{")) ? jsonBroker.ParseJson<cCoder.Data.Models.CMS.Page[]>(sanitizedData) : new cCoder.Data.Models.CMS.Page[1] { jsonBroker.ParseJson<cCoder.Data.Models.CMS.Page>(sanitizedData) });
        await pageOrchestrationService.ImportPagesAsync(appId, pages);
    }

    private async ValueTask ImportPageRolesAsync(int appId, PackageItem item)
    {
        ValidateAppId(appId, "appId");
        ValidatePackageItem(item, "item");
        PageRoleInfo[] pageRoles = ((!item.Data.StartsWith("{")) ? jsonBroker.ParseJson<PageRoleInfo[]>(item.Data) : new PageRoleInfo[1] { jsonBroker.ParseJson<PageRoleInfo>(item.Data) });
        await pageRoleOrchestrationService.ImportPageRolesAsync(appId, pageRoles);
    }

    private async ValueTask ImportResourcesAsync(int appId, PackageItem item)
    {
        ValidateAppId(appId, "appId");
        ValidatePackageItem(item, "item");
        string sanitizedData = RemoveComputedFields(item.Data);
        cCoder.Data.Models.CMS.Resource[] items = ((!sanitizedData.StartsWith("{")) ? jsonBroker.ParseJson<cCoder.Data.Models.CMS.Resource[]>(sanitizedData) : new cCoder.Data.Models.CMS.Resource[1] { jsonBroker.ParseJson<cCoder.Data.Models.CMS.Resource>(sanitizedData) });
        await resourceOrchestrationService.ImportResourcesAsync(appId, items);
    }

    private async ValueTask ImportScriptsAsync(int appId, PackageItem item)
    {
        ValidateAppId(appId, "appId");
        ValidatePackageItem(item, "item");
        string sanitizedData = RemoveComputedFields(item.Data);
        cCoder.Data.Models.CMS.Script[] items = ((!sanitizedData.StartsWith("{")) ? jsonBroker.ParseJson<cCoder.Data.Models.CMS.Script[]>(sanitizedData) : new cCoder.Data.Models.CMS.Script[1] { jsonBroker.ParseJson<cCoder.Data.Models.CMS.Script>(sanitizedData) });
        await scriptOrchestrationService.ImportScriptsAsync(appId, items);
    }

    private async ValueTask ImportTemplatesAsync(int appId, PackageItem item)
    {
        ValidateAppId(appId, "appId");
        ValidatePackageItem(item, "item");
        string sanitizedData = RemoveComputedFields(item.Data);
        cCoder.Data.Models.CMS.Template[] items = ((!sanitizedData.StartsWith("{")) ? jsonBroker.ParseJson<cCoder.Data.Models.CMS.Template[]>(sanitizedData) : new cCoder.Data.Models.CMS.Template[1] { jsonBroker.ParseJson<cCoder.Data.Models.CMS.Template>(sanitizedData) });
        await templateOrchestrationService.ImportTemplatesAsync(appId, items);
    }

    private static string RemoveComputedFields(string json)
    {
        if (string.IsNullOrWhiteSpace(json))
        {
            return json;
        }

        JToken token = JToken.Parse(json);
        RemoveComputedFields(token);
        return token.ToString();
    }

    private static void RemoveComputedFields(JToken token)
    {
        if (token is JObject jsonObject)
        {
            JProperty[] computedProperties = jsonObject.Properties()
                .Where(property => ComputedImportFields.Contains(property.Name))
                .ToArray();

            foreach (JProperty property in computedProperties)
            {
                property.Remove();
            }

            foreach (JProperty property in jsonObject.Properties().ToArray())
            {
                RemoveComputedFields(property.Value);
            }
        }
        else if (token is JArray jsonArray)
        {
            foreach (JToken arrayItem in jsonArray)
            {
                RemoveComputedFields(arrayItem);
            }
        }
    }

    private static int ValidateAppId(int appId, string parameterName)
    {
        if (appId < 1)
        {
            throw new ValidationException(parameterName + " must be greater than 0.");
        }
        return appId;
    }

    private static Package ValidatePackage(Package package, string parameterName)
    {
        if (package == null)
        {
            throw new ValidationException(parameterName + " is required.");
        }
        return package;
    }

    private static PackageItem ValidatePackageItem(PackageItem packageItem, string parameterName)
    {
        if (packageItem == null)
        {
            throw new ValidationException(parameterName + " is required.");
        }
        if (string.IsNullOrWhiteSpace(packageItem.Type))
        {
            throw new ValidationException(parameterName + ".Type is required.");
        }
        if (string.IsNullOrWhiteSpace(packageItem.Data))
        {
            throw new ValidationException(parameterName + ".Data is required.");
        }
        return packageItem;
    }
}
