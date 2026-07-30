// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.CMS;

namespace cCoder.ContentManagement.Brokers.Storages;

public interface ITemplateBroker
{
    IQueryable<Template> GetAllTemplates();

    IQueryable<Template> GetAllTemplatesIgnoringFilters();

    ValueTask<Template> AddTemplateAsync(Template newTemplate);

    ValueTask<Template> UpdateTemplateAsync(Template updatedTemplate);

    ValueTask<int> DeleteTemplateAsync(Template deletedTemplate);

    ValueTask DeleteAllTemplatesAsync(IEnumerable<Template> deletedTemplate);
}