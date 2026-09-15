// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Brokers;
using cCoder.ContentManagement.Brokers.Rendering;
using cCoder.ContentManagement.Brokers.Storages;
using cCoder.ContentManagement.Rendering.Brokers;
using cCoder.Data.Models.CMS;

namespace cCoder.ContentManagement.Tests.Brokers.Rendering;

internal sealed class TestContentRenderBroker(
    IMetadataReaderBroker metadataReaderBroker = null,
    ICommonObjectReaderBroker commonObjectReaderBroker = null,
    IWorkflowExecutionBroker workflowExecutionBroker = null,
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
        appBroker?.GetAllAppsIgnoringFilters().ToArray() ?? [];

    public Component[] GetComponents() =>
        componentBroker?.GetAllComponentsIgnoringFilters().ToArray() ?? [];

    public Resource[] GetResources() =>
        resourceBroker?.GetAllResourcesIgnoringFilters().ToArray() ?? [];

    public Script[] GetScripts() =>
        scriptBroker?.GetAllScriptsIgnoringFilters().ToArray() ?? [];

    public Template[] GetTemplates() =>
        templateBroker?.GetAllTemplatesIgnoringFilters().ToArray() ?? [];

    public Component GetComponent(int appId, string name) =>
        componentReaderBroker?.GetComponent(
            appId: appId,
            name: name);

    public Script GetScript(int appId, string name) =>
        scriptReaderBroker?.GetScript(
            appId: appId,
            name: name);

    public T GetCommonObject<T>(string key) =>
        commonObjectReaderBroker is null
            ? default
            : commonObjectReaderBroker.Get<T>(key: key);

    public string GetMetadata(string key, string culture) =>
        metadataReaderBroker?.Get(
            key: key,
            culture: culture);

    public string GetLatestTextContent(int appId, string path) =>
        renderFileContentBroker?.GetLatestTextContent(
            appId: appId,
            path: path);

    public string ExecuteWorkflow(string baseAddress, string content) =>
        workflowExecutionBroker?.Execute(
            baseAddress: baseAddress,
            content: content);
}