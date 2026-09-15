// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Dependencies;
using cCoder.Data.Models.CMS;

namespace cCoder.ContentManagement.Brokers.Rendering;

internal sealed class ContentRenderBroker(
    ContentRenderDependency contentRenderDependency)
        : IContentRenderBroker
{
    public App[] GetApps() =>
        contentRenderDependency.GetApps();

    public Component[] GetComponents() =>
        contentRenderDependency.GetComponents();

    public Resource[] GetResources() =>
        contentRenderDependency.GetResources();

    public Script[] GetScripts() =>
        contentRenderDependency.GetScripts();

    public Template[] GetTemplates() =>
        contentRenderDependency.GetTemplates();

    public Component GetComponent(int appId, string name) =>
        contentRenderDependency.GetComponent(
            appId: appId,
            name: name);

    public Script GetScript(int appId, string name) =>
        contentRenderDependency.GetScript(
            appId: appId,
            name: name);

    public T GetCommonObject<T>(string key) =>
        contentRenderDependency.GetCommonObject<T>(key: key);

    public string GetMetadata(string key, string culture) =>
        contentRenderDependency.GetMetadata(
            key: key,
            culture: culture);

    public string GetLatestTextContent(int appId, string path) =>
        contentRenderDependency.GetLatestTextContent(
            appId: appId,
            path: path);

    public string ExecuteWorkflow(string baseAddress, string content) =>
        contentRenderDependency.ExecuteWorkflow(
            baseAddress: baseAddress,
            content: content);
}