// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.CMS;

namespace cCoder.ContentManagement.Services.Foundations.Storages;

public interface ITemplateService
{
    Template GetTemplate(int templateId, bool ignoreFilters = false);

    IQueryable<Template> GetAllTemplate(bool ignoreFilters = false);

    ValueTask<Template> AddTemplateAsync(Template newTemplate);

    ValueTask<Template> UpdateTemplateAsync(Template updatedTemplate);

    ValueTask DeleteAsync(int templateId);
}