// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Models.Rendering;

namespace cCoder.ContentManagement.Services.Foundations.Rendering;

internal interface IComponentRenderService
{
    ComponentRenderFoundationOperation GetAppsComponentRenderFoundationOperation(ComponentRenderFoundationOperation componentRenderFoundationOperation);
    ComponentRenderFoundationOperation GetComponentsComponentRenderFoundationOperation(ComponentRenderFoundationOperation componentRenderFoundationOperation);
    ComponentRenderFoundationOperation GetResourcesComponentRenderFoundationOperation(ComponentRenderFoundationOperation componentRenderFoundationOperation);
    ComponentRenderFoundationOperation GetScriptsComponentRenderFoundationOperation(ComponentRenderFoundationOperation componentRenderFoundationOperation);
    ComponentRenderFoundationOperation GetComponentComponentRenderFoundationOperation(ComponentRenderFoundationOperation componentRenderFoundationOperation);
    ComponentRenderFoundationOperation GetScriptComponentRenderFoundationOperation(ComponentRenderFoundationOperation componentRenderFoundationOperation);
    ComponentRenderFoundationOperation GetResourceComponentRenderFoundationOperation(ComponentRenderFoundationOperation componentRenderFoundationOperation);
    ComponentRenderFoundationOperation GetMetadataComponentRenderFoundationOperation(ComponentRenderFoundationOperation componentRenderFoundationOperation);
    ComponentRenderFoundationOperation GetLatestTextContentComponentRenderFoundationOperation(ComponentRenderFoundationOperation componentRenderFoundationOperation);
    ComponentRenderFoundationOperation SerializeComponentRenderFoundationOperation(ComponentRenderFoundationOperation componentRenderFoundationOperation);
    ComponentRenderFoundationOperation SerializeIgnoringReferencesComponentRenderFoundationOperation(ComponentRenderFoundationOperation componentRenderFoundationOperation);
    ComponentRenderFoundationOperation ParseJsonComponentRenderFoundationOperation(ComponentRenderFoundationOperation componentRenderFoundationOperation);
    ComponentRenderFoundationOperation IsJsonObjectComponentRenderFoundationOperation(ComponentRenderFoundationOperation componentRenderFoundationOperation);
    ComponentRenderFoundationOperation IsJsonValueComponentRenderFoundationOperation(ComponentRenderFoundationOperation componentRenderFoundationOperation);
    ComponentRenderFoundationOperation GetJsonPropertiesComponentRenderFoundationOperation(ComponentRenderFoundationOperation componentRenderFoundationOperation);
    ComponentRenderFoundationOperation ExecuteWorkflowComponentRenderFoundationOperation(ComponentRenderFoundationOperation componentRenderFoundationOperation);
    ComponentRenderFoundationOperation ReplaceRegularExpressionComponentRenderFoundationOperation(ComponentRenderFoundationOperation componentRenderFoundationOperation);
    ComponentRenderFoundationOperation ForEachRegularExpressionMatchComponentRenderFoundationOperation(ComponentRenderFoundationOperation componentRenderFoundationOperation);
}