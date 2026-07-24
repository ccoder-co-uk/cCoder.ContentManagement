// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.CMS;
using cCoder.ContentManagement.Models;

namespace cCoder.ContentManagement.Services.Processings;

public interface ITemplateProcessingService
{
    Template GetTemplate(int templateId);

    IQueryable<Template> GetAllTemplate(bool ignoreFilters = false);

    ValueTask<Template> AddTemplateAsync(Template newTemplate);

    ValueTask<Template> UpdateTemplateAsync(Template updatedTemplate);

    ValueTask DeleteAsync(int templateId);

    ValueTask<IEnumerable<Result<Template>>> AddOrUpdateTemplateResult(IEnumerable<Template> newTemplate);

    ValueTask DeleteAllTemplateAsync(IEnumerable<Template> deletedTemplate);
}