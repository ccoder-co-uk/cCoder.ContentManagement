// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Brokers;
using cCoder.ContentManagement.Brokers.Loggings;
using cCoder.ContentManagement.Brokers.Storages;
using cCoder.ContentManagement.Models.Rendering;
using cCoder.ContentManagement.Models.Serialization;
using cCoder.ContentManagement.Rendering.Brokers;

namespace cCoder.ContentManagement.Services.Foundations.Rendering;

internal sealed partial class TemplateRenderService(
    IMetadataReaderBroker metadataReaderBroker,
    ICommonObjectReaderBroker commonObjectReaderBroker,
    IJsonBroker jsonBroker,
    ISystemTextJsonBroker systemTextJsonBroker,
    IWorkflowExecutionBroker workflowExecutionBroker,
    ILoggingBroker loggingBroker,
    IRegularExpressionBroker regularExpressionBroker,
    IAppBroker appBroker,
    IComponentBroker componentBroker,
    IResourceBroker resourceBroker,
    IScriptBroker scriptBroker,
    ITemplateBroker templateBroker)
        : ITemplateRenderService
{
    public TemplateRenderFoundationOperation GetAppsTemplateRenderFoundationOperation(
        TemplateRenderFoundationOperation templateRenderFoundationOperation) =>
        TryCatch(operation: () =>
    {
        ValidateAppsTemplateRenderFoundationOperationOnGet(
            inputs: [templateRenderFoundationOperation]);

        templateRenderFoundationOperation.Apps = appBroker
            .GetAllAppsIgnoringFilters()
            .ToArray();

        return templateRenderFoundationOperation;
    });

    public TemplateRenderFoundationOperation GetComponentsTemplateRenderFoundationOperation(
        TemplateRenderFoundationOperation templateRenderFoundationOperation) =>
        TryCatch(operation: () =>
    {
        ValidateComponentsTemplateRenderFoundationOperationOnGet(
            inputs: [templateRenderFoundationOperation]);

        templateRenderFoundationOperation.Components = componentBroker
            .GetAllComponentsIgnoringFilters()
            .ToArray();

        return templateRenderFoundationOperation;
    });

    public TemplateRenderFoundationOperation GetResourcesTemplateRenderFoundationOperation(
        TemplateRenderFoundationOperation templateRenderFoundationOperation) =>
        TryCatch(operation: () =>
    {
        ValidateResourcesTemplateRenderFoundationOperationOnGet(
            inputs: [templateRenderFoundationOperation]);

        templateRenderFoundationOperation.Resources = resourceBroker
            .GetAllResourcesIgnoringFilters()
            .ToArray();

        return templateRenderFoundationOperation;
    });

    public TemplateRenderFoundationOperation GetScriptsTemplateRenderFoundationOperation(
        TemplateRenderFoundationOperation templateRenderFoundationOperation) =>
        TryCatch(operation: () =>
    {
        ValidateScriptsTemplateRenderFoundationOperationOnGet(
            inputs: [templateRenderFoundationOperation]);

        templateRenderFoundationOperation.Scripts = scriptBroker
            .GetAllScriptsIgnoringFilters()
            .ToArray();

        return templateRenderFoundationOperation;
    });

    public TemplateRenderFoundationOperation GetTemplatesTemplateRenderFoundationOperation(
        TemplateRenderFoundationOperation templateRenderFoundationOperation) =>
        TryCatch(operation: () =>
    {
        ValidateTemplatesTemplateRenderFoundationOperationOnGet(
            inputs: [templateRenderFoundationOperation]);

        templateRenderFoundationOperation.Templates = templateBroker
            .GetAllTemplatesIgnoringFilters()
            .ToArray();

        return templateRenderFoundationOperation;
    });

    public TemplateRenderFoundationOperation GetComponentTemplateRenderFoundationOperation(
        TemplateRenderFoundationOperation templateRenderFoundationOperation) =>
        TryCatch(operation: () =>
    {
        ValidateComponentTemplateRenderFoundationOperationOnGet(
            inputs: [templateRenderFoundationOperation]);

        templateRenderFoundationOperation.Component =
            commonObjectReaderBroker.Get<cCoder.Data.Models.CMS.Component>(
                key: templateRenderFoundationOperation.Key);

        return templateRenderFoundationOperation;
    });

    public TemplateRenderFoundationOperation GetScriptTemplateRenderFoundationOperation(
        TemplateRenderFoundationOperation templateRenderFoundationOperation) =>
        TryCatch(operation: () =>
    {
        ValidateScriptTemplateRenderFoundationOperationOnGet(
            inputs: [templateRenderFoundationOperation]);

        templateRenderFoundationOperation.Script =
            commonObjectReaderBroker.Get<cCoder.Data.Models.CMS.Script>(
                key: templateRenderFoundationOperation.Key);

        return templateRenderFoundationOperation;
    });

    public TemplateRenderFoundationOperation GetResourceTemplateRenderFoundationOperation(
        TemplateRenderFoundationOperation templateRenderFoundationOperation) =>
        TryCatch(operation: () =>
    {
        ValidateResourceTemplateRenderFoundationOperationOnGet(
            inputs: [templateRenderFoundationOperation]);

        templateRenderFoundationOperation.Resource =
            commonObjectReaderBroker.Get<cCoder.Data.Models.CMS.Resource>(
                key: templateRenderFoundationOperation.Key);

        return templateRenderFoundationOperation;
    });

    public TemplateRenderFoundationOperation GetMetadataTemplateRenderFoundationOperation(
        TemplateRenderFoundationOperation templateRenderFoundationOperation) =>
        TryCatch(operation: () =>
    {
        ValidateMetadataTemplateRenderFoundationOperationOnGet(
            inputs: [templateRenderFoundationOperation]);

        templateRenderFoundationOperation.Content = metadataReaderBroker.Get(
            key: templateRenderFoundationOperation.Key,
            culture: templateRenderFoundationOperation.Culture);

        return templateRenderFoundationOperation;
    });

    public TemplateRenderFoundationOperation SerializeTemplateRenderFoundationOperation(
        TemplateRenderFoundationOperation templateRenderFoundationOperation) =>
        TryCatch(operation: () =>
    {
        ValidateTemplateRenderFoundationOperation(inputs: [templateRenderFoundationOperation]);
        templateRenderFoundationOperation.Content = jsonBroker.Serialize(value: templateRenderFoundationOperation.Value);
        return templateRenderFoundationOperation;
    });

    public TemplateRenderFoundationOperation SerializeIgnoringReferencesTemplateRenderFoundationOperation(
        TemplateRenderFoundationOperation templateRenderFoundationOperation) =>
        TryCatch(operation: () =>
    {
        ValidateTemplateRenderFoundationOperation(inputs: [templateRenderFoundationOperation]);
        templateRenderFoundationOperation.Content = jsonBroker.SerializeIgnoringReferences(value: templateRenderFoundationOperation.Value);
        return templateRenderFoundationOperation;
    });

    public TemplateRenderFoundationOperation ParseJsonTemplateRenderFoundationOperation(
        TemplateRenderFoundationOperation templateRenderFoundationOperation) =>
        TryCatch(operation: () =>
    {
        ValidateTemplateRenderFoundationOperation(inputs: [templateRenderFoundationOperation]);
        templateRenderFoundationOperation.Value = jsonBroker.ParseJson(json: templateRenderFoundationOperation.Content);
        return templateRenderFoundationOperation;
    });

    public TemplateRenderFoundationOperation NormalizeJsonTemplateRenderFoundationOperation(
        TemplateRenderFoundationOperation templateRenderFoundationOperation) =>
        TryCatch(operation: () =>
    {
        ValidateTemplateRenderFoundationOperation(inputs: [templateRenderFoundationOperation]);

        JsonValueDocument jsonValueDocument =
            systemTextJsonBroker.Normalize(value: templateRenderFoundationOperation.Value);

        templateRenderFoundationOperation.Value = jsonValueDocument.IsRawJson
            ? jsonBroker.ParseJson(json: jsonValueDocument.RawJson)
            : jsonValueDocument.Value;

        return templateRenderFoundationOperation;
    });

    public TemplateRenderFoundationOperation IsJsonObjectTemplateRenderFoundationOperation(
        TemplateRenderFoundationOperation templateRenderFoundationOperation) =>
        TryCatch(operation: () =>
    {
        ValidateTemplateRenderFoundationOperation(inputs: [templateRenderFoundationOperation]);
        templateRenderFoundationOperation.Condition = jsonBroker.IsJsonObject(value: templateRenderFoundationOperation.Value);
        return templateRenderFoundationOperation;
    });

    public TemplateRenderFoundationOperation IsJsonArrayTemplateRenderFoundationOperation(
        TemplateRenderFoundationOperation templateRenderFoundationOperation) =>
        TryCatch(operation: () =>
    {
        ValidateTemplateRenderFoundationOperation(inputs: [templateRenderFoundationOperation]);
        templateRenderFoundationOperation.Condition = jsonBroker.IsJsonArray(value: templateRenderFoundationOperation.Value);
        return templateRenderFoundationOperation;
    });

    public TemplateRenderFoundationOperation IsJsonValueTemplateRenderFoundationOperation(
        TemplateRenderFoundationOperation templateRenderFoundationOperation) =>
        TryCatch(operation: () =>
    {
        ValidateTemplateRenderFoundationOperation(inputs: [templateRenderFoundationOperation]);
        templateRenderFoundationOperation.Condition = jsonBroker.IsJsonValue(value: templateRenderFoundationOperation.Value);
        return templateRenderFoundationOperation;
    });

    public TemplateRenderFoundationOperation GetJsonPropertiesTemplateRenderFoundationOperation(
        TemplateRenderFoundationOperation templateRenderFoundationOperation) =>
        TryCatch(operation: () =>
    {
        ValidateJsonPropertiesTemplateRenderFoundationOperationOnGet(
            inputs: [templateRenderFoundationOperation]);

        templateRenderFoundationOperation.JsonProperties = jsonBroker
            .GetJsonProperties(value: templateRenderFoundationOperation.Value)
            .ToArray();

        return templateRenderFoundationOperation;
    });

    public TemplateRenderFoundationOperation ExecuteWorkflowTemplateRenderFoundationOperation(
        TemplateRenderFoundationOperation templateRenderFoundationOperation) =>
        TryCatch(operation: () =>
    {
        ValidateTemplateRenderFoundationOperation(inputs: [templateRenderFoundationOperation]);

        templateRenderFoundationOperation.Content = workflowExecutionBroker.Execute(
            baseAddress: templateRenderFoundationOperation.BaseAddress,
            content: templateRenderFoundationOperation.Content);

        return templateRenderFoundationOperation;
    });

    public TemplateRenderFoundationOperation IsLoggingEnabledTemplateRenderFoundationOperation(
        TemplateRenderFoundationOperation templateRenderFoundationOperation) =>
        TryCatch(operation: () =>
    {
        ValidateTemplateRenderFoundationOperation(inputs: [templateRenderFoundationOperation]);
        templateRenderFoundationOperation.Condition = loggingBroker.IsEnabled(logLevel: templateRenderFoundationOperation.LogLevel);
        return templateRenderFoundationOperation;
    });

    public TemplateRenderFoundationOperation LogDebugTemplateRenderFoundationOperation(
        TemplateRenderFoundationOperation templateRenderFoundationOperation) =>
        TryCatch(operation: () =>
    {
        ValidateTemplateRenderFoundationOperation(inputs: [templateRenderFoundationOperation]);
        loggingBroker.LogDebug(message: templateRenderFoundationOperation.Message, args: templateRenderFoundationOperation.Arguments ?? []);
        return templateRenderFoundationOperation;
    });

    public TemplateRenderFoundationOperation ReplaceRegularExpressionTemplateRenderFoundationOperation(
        TemplateRenderFoundationOperation templateRenderFoundationOperation) =>
        TryCatch(operation: () =>
    {
        ValidateTemplateRenderFoundationOperation(inputs: [templateRenderFoundationOperation]);

        templateRenderFoundationOperation.Content = regularExpressionBroker.Replace(
            input: templateRenderFoundationOperation.Input,
            pattern: templateRenderFoundationOperation.Pattern,
            evaluator: templateRenderFoundationOperation.Evaluator);

        return templateRenderFoundationOperation;
    });

    public TemplateRenderFoundationOperation ForEachRegularExpressionMatchTemplateRenderFoundationOperation(
        TemplateRenderFoundationOperation templateRenderFoundationOperation) =>
        TryCatch(operation: () =>
    {
        ValidateTemplateRenderFoundationOperation(inputs: [templateRenderFoundationOperation]);

        regularExpressionBroker.ForEachMatch(
            input: templateRenderFoundationOperation.Input,
            pattern: templateRenderFoundationOperation.Pattern,
            action: templateRenderFoundationOperation.MatchAction);

        return templateRenderFoundationOperation;
    });
}