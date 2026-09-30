// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.IO;
using System.Linq;
using System.Threading.Tasks;
using cCoder.CodeAnalysis.Exposures;
using cCoder.ContentManagement.Services.Aggregations;
using cCoder.Data.Models.CMS;

namespace cCoder.ContentManagement.Exposures;

internal sealed class TemplateManager(
    ITemplateManagerAggregationService templateManagerAggregationService)
        : ITemplateManager, ICompositionExposure
{
    public ValueTask<string> ReadContentAsync(Stream source) =>
        templateManagerAggregationService.ReadContentAsync(source: source);

    public byte[] ConvertHtmlToPdf(string html) =>
        templateManagerAggregationService.ConvertHtmlToPdf(html: html);

    public IQueryable<Template> GetAll() =>
        templateManagerAggregationService.GetAllTemplates();

    public Template Get(int templateId) =>
        templateManagerAggregationService.GetTemplate(templateId: templateId);

    public ValueTask<Template> AddAsync(Template newTemplate) =>
        templateManagerAggregationService.AddTemplateAsync(newTemplate: newTemplate);

    public ValueTask<Template> UpdateAsync(Template updatedTemplate) =>
        templateManagerAggregationService.UpdateTemplateAsync(updatedTemplate: updatedTemplate);

    public ValueTask DeleteAsync(int templateId) =>
        templateManagerAggregationService.DeleteTemplateAsync(templateId: templateId);

    public ValueTask ImportTemplatesAsync(int appId, Template[] items) =>
        templateManagerAggregationService.ImportTemplatesAsync(
            appId: appId,
            items: items);
}