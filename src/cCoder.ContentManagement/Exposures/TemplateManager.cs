// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Services.Orchestrations;
using cCoder.Data.Models.CMS;

namespace cCoder.ContentManagement.Exposures;

internal sealed class TemplateManager(
    ITemplateOrchestrationService templateOrchestrationService)
        : ITemplateManager
{
    public ValueTask<string> ReadContentAsync(Stream source) =>
        templateOrchestrationService.ReadContentAsync(source: source);

    public byte[] ConvertHtmlToPdf(string html) =>
        templateOrchestrationService.ConvertHtmlToPdf(html: html);

    public IQueryable<Template> GetAll() =>
        templateOrchestrationService.GetAllTemplate();

    public Template Get(int templateId) =>
        templateOrchestrationService.GetTemplate(templateId: templateId);

    public ValueTask<Template> AddAsync(Template newTemplate) =>
        templateOrchestrationService.AddTemplateAsync(newTemplate: newTemplate);

    public ValueTask<Template> UpdateAsync(Template updatedTemplate) =>
        templateOrchestrationService.UpdateTemplateAsync(updatedTemplate: updatedTemplate);

    public ValueTask DeleteAsync(int templateId) =>
        templateOrchestrationService.DeleteAsync(templateId: templateId);

    public ValueTask ImportTemplatesAsync(int appId, Template[] items) =>
        templateOrchestrationService.ImportTemplatesAsync(
            appId: appId,
            items: items);
}