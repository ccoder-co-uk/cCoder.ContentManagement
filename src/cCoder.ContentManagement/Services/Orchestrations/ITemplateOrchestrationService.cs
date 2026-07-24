// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.CMS;
using cCoder.ContentManagement.Models;

namespace cCoder.ContentManagement.Services.Orchestrations;

public interface ITemplateOrchestrationService
{
    Template GetTemplate(int templateId);

    IQueryable<Template> GetAllTemplate(bool ignoreFilters = false);

    ValueTask<Template> AddTemplateAsync(Template newTemplate);

    ValueTask<Template> UpdateTemplateAsync(Template updatedTemplate);

    ValueTask DeleteAsync(int templateId);

    ValueTask DeleteByAppIdAsync(int appId);

    ValueTask<IEnumerable<OperationResult<Template>>> AddOrUpdateTemplateResult(IEnumerable<Template> newTemplate);

    ValueTask ImportTemplatesAsync(int appId, Template[] items);

    ValueTask DeleteAllTemplateAsync(IEnumerable<Template> deletedTemplate);
}