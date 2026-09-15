// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Services.Orchestrations;
using cCoder.Data.Models.CMS;

namespace cCoder.ContentManagement.Services.Aggregations;

internal sealed partial class TemplateManagerAggregationService(
    ITemplateOrchestrationService templateOrchestrationService,
    ITemplateContentOrchestrationService templateContentOrchestrationService)
        : ITemplateManagerAggregationService
{
    public ValueTask<string> ReadContentAsync(Stream source) =>
        TryCatch(operation: () =>
        {
            ValidateTemplateContentOnRead(inputs: [source]);
            return templateContentOrchestrationService.ReadContentAsync(source: source);
        }, isValueTask: true);

    public byte[] ConvertHtmlToPdf(string html) =>
        TryCatch(operation: () =>
        {
            ValidateHtmlOnConvert(inputs: [html]);
            return templateContentOrchestrationService.ConvertHtmlToPdf(html: html);
        });

    public IQueryable<Template> GetAllTemplate() =>
        TryCatch(operation: () =>
        {
            return templateOrchestrationService.GetAllTemplate();
        });

    public Template GetTemplate(int templateId) =>
        TryCatch(operation: () =>
        {
            ValidateTemplateOnGet(inputs: [templateId]);
            return templateOrchestrationService.GetTemplate(templateId: templateId);
        });

    public ValueTask<Template> AddTemplateAsync(Template newTemplate) =>
        TryCatch(operation: () =>
        {
            ValidateTemplateOnAdd(inputs: [newTemplate]);
            return templateOrchestrationService.AddTemplateAsync(newTemplate: newTemplate);
        }, isValueTask: true);

    public ValueTask<Template> UpdateTemplateAsync(Template updatedTemplate) =>
        TryCatch(operation: () =>
        {
            ValidateTemplateOnUpdate(inputs: [updatedTemplate]);
            return templateOrchestrationService.UpdateTemplateAsync(updatedTemplate: updatedTemplate);
        }, isValueTask: true);

    public ValueTask DeleteTemplateAsync(int templateId) =>
        TryCatch(operation: () =>
        {
            ValidateTemplateOnDelete(inputs: [templateId]);
            return templateOrchestrationService.DeleteAsync(templateId: templateId);
        }, isValueTask: true);

    public ValueTask ImportTemplatesAsync(int appId, Template[] items) =>
        TryCatch(operation: () =>
        {
            ValidateTemplatesOnImport(inputs: [appId, items]);
            return templateOrchestrationService.ImportTemplatesAsync(appId: appId, items: items);
        }, isValueTask: true);
}