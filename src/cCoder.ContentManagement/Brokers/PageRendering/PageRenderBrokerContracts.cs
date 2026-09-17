// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------


using cCoder.ContentManagement.Models.PageRendering;
using cCoder.Data.Models;
using cCoder.Data.Models.CMS;

namespace cCoder.ContentManagement.Rendering.Brokers;

internal interface IComponentReaderBroker
{
    IEnumerable<Component> GetComponents(int appId);

    Component GetComponent(int appId, string name);
}

internal interface IScriptReaderBroker
{
    IEnumerable<Script> GetScripts(int appId);

    Script GetScript(int appId, string name);
}