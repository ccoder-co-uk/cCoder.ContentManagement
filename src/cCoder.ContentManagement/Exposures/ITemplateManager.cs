// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.CMS;

namespace cCoder.ContentManagement.Exposures;

public interface ITemplateManager
{
    string Render(int appId, string name, string culture, dynamic model);

    IQueryable<Template> GetAll();

    Template Get(int templateId);

    ValueTask<Template> AddAsync(Template newTemplate);

    ValueTask<Template> UpdateAsync(Template updatedTemplate);

    ValueTask DeleteAsync(int templateId);
}