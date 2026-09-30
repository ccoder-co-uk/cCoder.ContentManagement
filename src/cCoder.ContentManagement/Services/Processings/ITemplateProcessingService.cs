// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Models.Results;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using cCoder.Data.Models.CMS;

namespace cCoder.ContentManagement.Services.Processings;

internal interface ITemplateProcessingService
{
    Template GetTemplate(int templateId);

    IQueryable<Template> GetAllTemplates(bool ignoreFilters = false);

    ValueTask<Template> AddTemplateAsync(Template newTemplate);

    ValueTask<Template> UpdateTemplateAsync(Template updatedTemplate);

    ValueTask DeleteAsync(int templateId);

    ValueTask<IEnumerable<OperationResult<Template>>> AddOrUpdateTemplateResult(IEnumerable<Template> newTemplate);

    ValueTask DeleteAllTemplateAsync(IEnumerable<Template> deletedTemplate);
}