// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Brokers;
using cCoder.ContentManagement.Models.Rendering;
using cCoder.ContentManagement.Rendering.Brokers;
using cCoder.ContentManagement.Brokers.Storages;

namespace cCoder.ContentManagement.Services.Foundations.Rendering;

internal sealed partial class ComponentRenderService(
    IMetadataReaderBroker metadataReaderBroker,
    ICommonObjectReaderBroker commonObjectReaderBroker,
    IJsonBroker jsonBroker,
    IWorkflowExecutionBroker workflowExecutionBroker,
    IRegularExpressionBroker regularExpressionBroker,
    IRenderFileContentBroker renderFileContentBroker,
    IAppBroker appBroker,
    IComponentBroker componentBroker,
    IResourceBroker resourceBroker,
    IScriptBroker scriptBroker)
        : IComponentRenderService
{
    public ComponentRenderFoundationOperation GetAppsComponentRenderFoundationOperation(
        ComponentRenderFoundationOperation componentRenderFoundationOperation) =>
        TryCatch(operation: () =>
    {
        ValidateAppsComponentRenderFoundationOperationOnGet(inputs: [componentRenderFoundationOperation]);

        componentRenderFoundationOperation.Apps = appBroker
            .GetAllAppsIgnoringFilters()
            .ToArray();

        return componentRenderFoundationOperation;
    });

    public ComponentRenderFoundationOperation GetComponentsComponentRenderFoundationOperation(
        ComponentRenderFoundationOperation componentRenderFoundationOperation) =>
        TryCatch(operation: () =>
    {
        ValidateComponentsComponentRenderFoundationOperationOnGet(inputs: [componentRenderFoundationOperation]);

        componentRenderFoundationOperation.Components = componentBroker
            .GetAllComponentsIgnoringFilters()
            .ToArray();

        return componentRenderFoundationOperation;
    });

    public ComponentRenderFoundationOperation GetResourcesComponentRenderFoundationOperation(
        ComponentRenderFoundationOperation componentRenderFoundationOperation) =>
        TryCatch(operation: () =>
    {
        ValidateResourcesComponentRenderFoundationOperationOnGet(inputs: [componentRenderFoundationOperation]);

        componentRenderFoundationOperation.Resources = resourceBroker
            .GetAllResourcesIgnoringFilters()
            .ToArray();

        return componentRenderFoundationOperation;
    });

    public ComponentRenderFoundationOperation GetScriptsComponentRenderFoundationOperation(
        ComponentRenderFoundationOperation componentRenderFoundationOperation) =>
        TryCatch(operation: () =>
    {
        ValidateScriptsComponentRenderFoundationOperationOnGet(inputs: [componentRenderFoundationOperation]);

        componentRenderFoundationOperation.Scripts = scriptBroker
            .GetAllScriptsIgnoringFilters()
            .ToArray();

        return componentRenderFoundationOperation;
    });

    public ComponentRenderFoundationOperation GetComponentComponentRenderFoundationOperation(
        ComponentRenderFoundationOperation componentRenderFoundationOperation) =>
        TryCatch(operation: () =>
    {
        ValidateComponentComponentRenderFoundationOperationOnGet(inputs: [componentRenderFoundationOperation]);
        componentRenderFoundationOperation.Component = commonObjectReaderBroker.Get<cCoder.Data.Models.CMS.Component>(key: componentRenderFoundationOperation.Key);
        return componentRenderFoundationOperation;
    });

    public ComponentRenderFoundationOperation GetScriptComponentRenderFoundationOperation(
        ComponentRenderFoundationOperation componentRenderFoundationOperation) =>
        TryCatch(operation: () =>
    {
        ValidateScriptComponentRenderFoundationOperationOnGet(inputs: [componentRenderFoundationOperation]);
        componentRenderFoundationOperation.Script = commonObjectReaderBroker.Get<cCoder.Data.Models.CMS.Script>(key: componentRenderFoundationOperation.Key);
        return componentRenderFoundationOperation;
    });

    public ComponentRenderFoundationOperation GetResourceComponentRenderFoundationOperation(
        ComponentRenderFoundationOperation componentRenderFoundationOperation) =>
        TryCatch(operation: () =>
    {
        ValidateResourceComponentRenderFoundationOperationOnGet(inputs: [componentRenderFoundationOperation]);
        componentRenderFoundationOperation.Resource = commonObjectReaderBroker.Get<cCoder.Data.Models.CMS.Resource>(key: componentRenderFoundationOperation.Key);
        return componentRenderFoundationOperation;
    });

    public ComponentRenderFoundationOperation GetMetadataComponentRenderFoundationOperation(
        ComponentRenderFoundationOperation componentRenderFoundationOperation) =>
        TryCatch(operation: () =>
    {
        ValidateMetadataComponentRenderFoundationOperationOnGet(inputs: [componentRenderFoundationOperation]);
        componentRenderFoundationOperation.Content = metadataReaderBroker.Get(key: componentRenderFoundationOperation.Key, culture: componentRenderFoundationOperation.Culture);
        return componentRenderFoundationOperation;
    });

    public ComponentRenderFoundationOperation GetLatestTextContentComponentRenderFoundationOperation(
        ComponentRenderFoundationOperation componentRenderFoundationOperation) =>
        TryCatch(operation: () =>
    {
        ValidateLatestTextContentComponentRenderFoundationOperationOnGet(inputs: [componentRenderFoundationOperation]);
        ValidateAppId(appId: componentRenderFoundationOperation.AppId, parameterName: "componentRenderFoundationOperation.AppId");
        ValidatePath(path: componentRenderFoundationOperation.Path, parameterName: "componentRenderFoundationOperation.Path");

        componentRenderFoundationOperation.Content =
            renderFileContentBroker.GetLatestTextContent(
                appId: componentRenderFoundationOperation.AppId,
                path: componentRenderFoundationOperation.Path) ?? string.Empty;
        return componentRenderFoundationOperation;
    });

    public ComponentRenderFoundationOperation SerializeComponentRenderFoundationOperation(
        ComponentRenderFoundationOperation componentRenderFoundationOperation) =>
        TryCatch(operation: () =>
    {
        ValidateComponentRenderFoundationOperation(inputs: [componentRenderFoundationOperation]);
        componentRenderFoundationOperation.Content = jsonBroker.Serialize(value: componentRenderFoundationOperation.Value);
        return componentRenderFoundationOperation;
    });

    public ComponentRenderFoundationOperation SerializeIgnoringReferencesComponentRenderFoundationOperation(
        ComponentRenderFoundationOperation componentRenderFoundationOperation) =>
        TryCatch(operation: () =>
    {
        ValidateComponentRenderFoundationOperation(inputs: [componentRenderFoundationOperation]);
        componentRenderFoundationOperation.Content = jsonBroker.SerializeIgnoringReferences(value: componentRenderFoundationOperation.Value);
        return componentRenderFoundationOperation;
    });

    public ComponentRenderFoundationOperation ParseJsonComponentRenderFoundationOperation(
        ComponentRenderFoundationOperation componentRenderFoundationOperation) =>
        TryCatch(operation: () =>
    {
        ValidateComponentRenderFoundationOperation(inputs: [componentRenderFoundationOperation]);
        componentRenderFoundationOperation.Value = jsonBroker.ParseJson(json: componentRenderFoundationOperation.Content);
        return componentRenderFoundationOperation;
    });

    public ComponentRenderFoundationOperation IsJsonObjectComponentRenderFoundationOperation(
        ComponentRenderFoundationOperation componentRenderFoundationOperation) =>
        TryCatch(operation: () =>
    {
        ValidateComponentRenderFoundationOperation(inputs: [componentRenderFoundationOperation]);
        componentRenderFoundationOperation.Condition = jsonBroker.IsJsonObject(value: componentRenderFoundationOperation.Value);
        return componentRenderFoundationOperation;
    });

    public ComponentRenderFoundationOperation IsJsonValueComponentRenderFoundationOperation(
        ComponentRenderFoundationOperation componentRenderFoundationOperation) =>
        TryCatch(operation: () =>
    {
        ValidateComponentRenderFoundationOperation(inputs: [componentRenderFoundationOperation]);
        componentRenderFoundationOperation.Condition = jsonBroker.IsJsonValue(value: componentRenderFoundationOperation.Value);
        return componentRenderFoundationOperation;
    });

    public ComponentRenderFoundationOperation GetJsonPropertiesComponentRenderFoundationOperation(
        ComponentRenderFoundationOperation componentRenderFoundationOperation) =>
        TryCatch(operation: () =>
    {
        ValidateJsonPropertiesComponentRenderFoundationOperationOnGet(inputs: [componentRenderFoundationOperation]);

        componentRenderFoundationOperation.JsonProperties = jsonBroker
            .GetJsonProperties(value: componentRenderFoundationOperation.Value)
            .ToArray();

        return componentRenderFoundationOperation;
    });

    public ComponentRenderFoundationOperation ExecuteWorkflowComponentRenderFoundationOperation(
        ComponentRenderFoundationOperation componentRenderFoundationOperation) =>
        TryCatch(operation: () =>
    {
        ValidateComponentRenderFoundationOperation(inputs: [componentRenderFoundationOperation]);
        componentRenderFoundationOperation.Content = workflowExecutionBroker.Execute(baseAddress: componentRenderFoundationOperation.BaseAddress, content: componentRenderFoundationOperation.Content);
        return componentRenderFoundationOperation;
    });

    public ComponentRenderFoundationOperation ReplaceRegularExpressionComponentRenderFoundationOperation(
        ComponentRenderFoundationOperation componentRenderFoundationOperation) =>
        TryCatch(operation: () =>
    {
        ValidateComponentRenderFoundationOperation(inputs: [componentRenderFoundationOperation]);
        componentRenderFoundationOperation.Content = regularExpressionBroker.Replace(input: componentRenderFoundationOperation.Input, pattern: componentRenderFoundationOperation.Pattern, evaluator: componentRenderFoundationOperation.Evaluator);
        return componentRenderFoundationOperation;
    });

    public ComponentRenderFoundationOperation ForEachRegularExpressionMatchComponentRenderFoundationOperation(
        ComponentRenderFoundationOperation componentRenderFoundationOperation) =>
        TryCatch(operation: () =>
    {
        ValidateComponentRenderFoundationOperation(inputs: [componentRenderFoundationOperation]);
        regularExpressionBroker.ForEachMatch(input: componentRenderFoundationOperation.Input, pattern: componentRenderFoundationOperation.Pattern, action: componentRenderFoundationOperation.MatchAction);
        return componentRenderFoundationOperation;
    });
}