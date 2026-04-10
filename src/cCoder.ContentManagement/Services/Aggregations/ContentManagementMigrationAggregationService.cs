using System.ComponentModel.DataAnnotations;
using cCoder.ContentManagement.Brokers;
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
        cCoder.Data.Models.CMS.Component[] items = ((!item.Data.StartsWith("{")) ? jsonBroker.ParseJson<cCoder.Data.Models.CMS.Component[]>(item.Data) : new cCoder.Data.Models.CMS.Component[1] { jsonBroker.ParseJson<cCoder.Data.Models.CMS.Component>(item.Data) });
        await componentOrchestrationService.ImportComponentsAsync(appId, items);
    }

    private async ValueTask ImportLayoutsAsync(int appId, PackageItem item)
    {
        ValidateAppId(appId, "appId");
        ValidatePackageItem(item, "item");
        cCoder.Data.Models.CMS.Layout[] items = ((!item.Data.StartsWith("{")) ? jsonBroker.ParseJson<cCoder.Data.Models.CMS.Layout[]>(item.Data) : new cCoder.Data.Models.CMS.Layout[1] { jsonBroker.ParseJson<cCoder.Data.Models.CMS.Layout>(item.Data) });
        await layoutOrchestrationService.ImportLayoutsAsync(appId, items);
    }

    private async ValueTask ImportPagesAsync(int appId, PackageItem item)
    {
        ValidateAppId(appId, "appId");
        ValidatePackageItem(item, "item");
        cCoder.Data.Models.CMS.Page[] pages = ((!item.Data.StartsWith("{")) ? jsonBroker.ParseJson<cCoder.Data.Models.CMS.Page[]>(item.Data) : new cCoder.Data.Models.CMS.Page[1] { jsonBroker.ParseJson<cCoder.Data.Models.CMS.Page>(item.Data) });
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
        cCoder.Data.Models.CMS.Resource[] items = ((!item.Data.StartsWith("{")) ? jsonBroker.ParseJson<cCoder.Data.Models.CMS.Resource[]>(item.Data) : new cCoder.Data.Models.CMS.Resource[1] { jsonBroker.ParseJson<cCoder.Data.Models.CMS.Resource>(item.Data) });
        await resourceOrchestrationService.ImportResourcesAsync(appId, items);
    }

    private async ValueTask ImportScriptsAsync(int appId, PackageItem item)
    {
        ValidateAppId(appId, "appId");
        ValidatePackageItem(item, "item");
        cCoder.Data.Models.CMS.Script[] items = ((!item.Data.StartsWith("{")) ? jsonBroker.ParseJson<cCoder.Data.Models.CMS.Script[]>(item.Data) : new cCoder.Data.Models.CMS.Script[1] { jsonBroker.ParseJson<cCoder.Data.Models.CMS.Script>(item.Data) });
        await scriptOrchestrationService.ImportScriptsAsync(appId, items);
    }

    private async ValueTask ImportTemplatesAsync(int appId, PackageItem item)
    {
        ValidateAppId(appId, "appId");
        ValidatePackageItem(item, "item");
        cCoder.Data.Models.CMS.Template[] items = ((!item.Data.StartsWith("{")) ? jsonBroker.ParseJson<cCoder.Data.Models.CMS.Template[]>(item.Data) : new cCoder.Data.Models.CMS.Template[1] { jsonBroker.ParseJson<cCoder.Data.Models.CMS.Template>(item.Data) });
        await templateOrchestrationService.ImportTemplatesAsync(appId, items);
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
