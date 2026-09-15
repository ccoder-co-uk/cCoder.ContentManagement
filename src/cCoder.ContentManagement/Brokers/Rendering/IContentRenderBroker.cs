// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.CMS;

namespace cCoder.ContentManagement.Brokers.Rendering;

internal interface IContentRenderBroker
{
    App[] GetApps();

    Component[] GetComponents();

    Resource[] GetResources();

    Script[] GetScripts();

    Template[] GetTemplates();

    Component GetComponent(int appId, string name);

    Script GetScript(int appId, string name);

    T GetCommonObject<T>(string key);

    string GetMetadata(string key, string culture);

    string GetLatestTextContent(int appId, string path);

    string ExecuteWorkflow(string baseAddress, string content);
}