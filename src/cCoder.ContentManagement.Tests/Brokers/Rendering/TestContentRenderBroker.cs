// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Brokers.Rendering;
using cCoder.ContentManagement.Brokers.Storages;
using cCoder.ContentManagement.Rendering.Brokers;
using cCoder.Data.Models.CMS;

namespace cCoder.ContentManagement.Tests.Brokers.Rendering;

#pragma warning disable STXTEST001
internal sealed class TestContentRenderBroker(
    IRenderFileContentBroker renderFileContentBroker = null,
    IAppBroker appBroker = null,
    IComponentBroker componentBroker = null,
    IResourceBroker resourceBroker = null,
    IScriptBroker scriptBroker = null,
    ITemplateBroker templateBroker = null,
    IComponentReaderBroker componentReaderBroker = null,
    IScriptReaderBroker scriptReaderBroker = null)
        : IContentRenderBroker
{
    public App[] GetApps() =>
        appBroker?.GetAllAppsIgnoringFilters()
            .ToArray() ?? [];

    public Component[] GetComponents() =>
        componentBroker?.GetAllComponentsIgnoringFilters()
            .ToArray() ?? [];

    public Resource[] GetResources() =>
        resourceBroker?.GetAllResourcesIgnoringFilters()
            .ToArray() ?? [];

    public Script[] GetScripts() =>
        scriptBroker?.GetAllScriptsIgnoringFilters()
            .ToArray() ?? [];

    public Template[] GetTemplates() =>
        templateBroker?.GetAllTemplatesIgnoringFilters()
            .ToArray() ?? [];

    public Component GetComponent(int appId, string name) =>
        componentReaderBroker?.GetComponent(
            appId: appId,
            name: name);

    public Script GetScript(int appId, string name) =>
        scriptReaderBroker?.GetScript(
            appId: appId,
            name: name);

    public string GetLatestTextContent(int appId, string path) =>
        renderFileContentBroker?.GetLatestTextContent(
            appId: appId,
            path: path);
}
#pragma warning restore STXTEST001