// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Models.Rendering;

namespace cCoder.ContentManagement.Services.Foundations.Rendering;

internal interface ITemplateRenderService
{
    TemplateRenderFoundationOperation GetPropertyValuesTemplateRenderFoundationOperation(
        TemplateRenderFoundationOperation templateRenderFoundationOperation);
    TemplateRenderFoundationOperation GetAppsTemplateRenderFoundationOperation(TemplateRenderFoundationOperation templateRenderFoundationOperation);
    TemplateRenderFoundationOperation GetComponentsTemplateRenderFoundationOperation(TemplateRenderFoundationOperation templateRenderFoundationOperation);
    TemplateRenderFoundationOperation GetResourcesTemplateRenderFoundationOperation(TemplateRenderFoundationOperation templateRenderFoundationOperation);
    TemplateRenderFoundationOperation GetScriptsTemplateRenderFoundationOperation(TemplateRenderFoundationOperation templateRenderFoundationOperation);
    TemplateRenderFoundationOperation GetTemplatesTemplateRenderFoundationOperation(TemplateRenderFoundationOperation templateRenderFoundationOperation);
    TemplateRenderFoundationOperation GetComponentTemplateRenderFoundationOperation(TemplateRenderFoundationOperation templateRenderFoundationOperation);
    TemplateRenderFoundationOperation GetScriptTemplateRenderFoundationOperation(TemplateRenderFoundationOperation templateRenderFoundationOperation);
    TemplateRenderFoundationOperation GetResourceTemplateRenderFoundationOperation(TemplateRenderFoundationOperation templateRenderFoundationOperation);
    TemplateRenderFoundationOperation GetMetadataTemplateRenderFoundationOperation(TemplateRenderFoundationOperation templateRenderFoundationOperation);
    TemplateRenderFoundationOperation SerializeTemplateRenderFoundationOperation(TemplateRenderFoundationOperation templateRenderFoundationOperation);
    TemplateRenderFoundationOperation SerializeIgnoringReferencesTemplateRenderFoundationOperation(TemplateRenderFoundationOperation templateRenderFoundationOperation);
    TemplateRenderFoundationOperation ParseJsonTemplateRenderFoundationOperation(TemplateRenderFoundationOperation templateRenderFoundationOperation);
    TemplateRenderFoundationOperation NormalizeJsonTemplateRenderFoundationOperation(TemplateRenderFoundationOperation templateRenderFoundationOperation);
    TemplateRenderFoundationOperation IsJsonObjectTemplateRenderFoundationOperation(TemplateRenderFoundationOperation templateRenderFoundationOperation);
    TemplateRenderFoundationOperation IsJsonArrayTemplateRenderFoundationOperation(TemplateRenderFoundationOperation templateRenderFoundationOperation);
    TemplateRenderFoundationOperation IsJsonValueTemplateRenderFoundationOperation(TemplateRenderFoundationOperation templateRenderFoundationOperation);
    TemplateRenderFoundationOperation GetJsonPropertiesTemplateRenderFoundationOperation(TemplateRenderFoundationOperation templateRenderFoundationOperation);
    TemplateRenderFoundationOperation ExecuteWorkflowTemplateRenderFoundationOperation(TemplateRenderFoundationOperation templateRenderFoundationOperation);
    TemplateRenderFoundationOperation IsLoggingEnabledTemplateRenderFoundationOperation(TemplateRenderFoundationOperation templateRenderFoundationOperation);
    TemplateRenderFoundationOperation LogDebugTemplateRenderFoundationOperation(TemplateRenderFoundationOperation templateRenderFoundationOperation);
    TemplateRenderFoundationOperation ReplaceRegularExpressionTemplateRenderFoundationOperation(TemplateRenderFoundationOperation templateRenderFoundationOperation);
    TemplateRenderFoundationOperation ForEachRegularExpressionMatchTemplateRenderFoundationOperation(TemplateRenderFoundationOperation templateRenderFoundationOperation);
}