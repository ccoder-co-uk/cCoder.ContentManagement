// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.CMS;

namespace cCoder.ContentManagement.Services.Aggregations;

internal interface ITemplateManagerAggregationService
{
    ValueTask<string> ReadContentAsync(Stream source);
    byte[] ConvertHtmlToPdf(string html);
    IQueryable<Template> GetAllTemplate();
    Template GetTemplate(int templateId);
    ValueTask<Template> AddTemplateAsync(Template newTemplate);
    ValueTask<Template> UpdateTemplateAsync(Template updatedTemplate);
    ValueTask DeleteTemplateAsync(int templateId);
    ValueTask ImportTemplatesAsync(int appId, Template[] items);
}