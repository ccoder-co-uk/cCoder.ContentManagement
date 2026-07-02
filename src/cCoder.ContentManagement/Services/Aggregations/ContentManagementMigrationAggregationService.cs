using System.ComponentModel.DataAnnotations;
using cCoder.ContentManagement.Brokers;
using Newtonsoft.Json.Linq;
using cCoder.ContentManagement.Services.Orchestrations;
using cCoder.ContentManagement.Models;
using cCoder.Data.Models.Packaging;
using cCoder.Data.Models.CMS;

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

        if (package.Items != null)
            foreach (PackageItem item in package.Items)
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
        Component[] items = ((!sanitizedData.StartsWith("{")) ? jsonBroker.ParseJson<Component[]>(sanitizedData) : new Component[1] { jsonBroker.ParseJson<Component>(sanitizedData) });
        await componentOrchestrationService.ImportComponentsAsync(appId, items);
    }

    private async ValueTask ImportLayoutsAsync(int appId, PackageItem item)
    {
        ValidateAppId(appId, "appId");
        ValidatePackageItem(item, "item");
        string sanitizedData = RemoveComputedFields(item.Data);
        Layout[] items = ((!sanitizedData.StartsWith("{")) ? jsonBroker.ParseJson<Layout[]>(sanitizedData) : new Layout[1] { jsonBroker.ParseJson<Layout>(sanitizedData) });
        await layoutOrchestrationService.ImportLayoutsAsync(appId, items);
    }

    private async ValueTask ImportPagesAsync(int appId, PackageItem item)
    {
        ValidateAppId(appId, "appId");
        ValidatePackageItem(item, "item");
        string sanitizedData = RemoveComputedFields(item.Data);
        Page[] pages = ((!sanitizedData.StartsWith("{")) ? jsonBroker.ParseJson<Page[]>(sanitizedData) : new Page[1] { jsonBroker.ParseJson<Page>(sanitizedData) });
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
        Resource[] items = ((!sanitizedData.StartsWith("{")) ? jsonBroker.ParseJson<Resource[]>(sanitizedData) : new Resource[1] { jsonBroker.ParseJson<Resource>(sanitizedData) });
        await resourceOrchestrationService.ImportResourcesAsync(appId, items);
    }

    private async ValueTask ImportScriptsAsync(int appId, PackageItem item)
    {
        ValidateAppId(appId, "appId");
        ValidatePackageItem(item, "item");
        string sanitizedData = RemoveComputedFields(item.Data);
        Script[] items = ((!sanitizedData.StartsWith("{")) ? jsonBroker.ParseJson<Script[]>(sanitizedData) : new Script[1] { jsonBroker.ParseJson<Script>(sanitizedData) });
        await scriptOrchestrationService.ImportScriptsAsync(appId, items);
    }

    private async ValueTask ImportTemplatesAsync(int appId, PackageItem item)
    {
        ValidateAppId(appId, "appId");
        ValidatePackageItem(item, "item");
        string sanitizedData = RemoveComputedFields(item.Data);
        Template[] items = ((!sanitizedData.StartsWith("{")) ? jsonBroker.ParseJson<Template[]>(sanitizedData) : new Template[1] { jsonBroker.ParseJson<Template>(sanitizedData) });
        await templateOrchestrationService.ImportTemplatesAsync(appId, items);
    }

    private static string RemoveComputedFields(string json)
    {
        if (string.IsNullOrWhiteSpace(json))
            return json;

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
                property.Remove();

            foreach (JProperty property in jsonObject.Properties().ToArray())
                RemoveComputedFields(property.Value);
        }
        else if (token is JArray jsonArray)
        {
            foreach (JToken arrayItem in jsonArray)
                RemoveComputedFields(arrayItem);
        }
    }

    private static int ValidateAppId(int appId, string parameterName)
    {
        if (appId < 1)
            throw new ValidationException(parameterName + " must be greater than 0.");

        return appId;
    }

    private static Package ValidatePackage(Package package, string parameterName)
    {
        if (package == null)
            throw new ValidationException(parameterName + " is required.");

        return package;
    }

    private static PackageItem ValidatePackageItem(PackageItem packageItem, string parameterName)
    {
        if (packageItem == null)
            throw new ValidationException(parameterName + " is required.");

        if (string.IsNullOrWhiteSpace(packageItem.Type))
            throw new ValidationException(parameterName + ".Type is required.");

        if (string.IsNullOrWhiteSpace(packageItem.Data))
            throw new ValidationException(parameterName + ".Data is required.");

        return packageItem;
    }
}
