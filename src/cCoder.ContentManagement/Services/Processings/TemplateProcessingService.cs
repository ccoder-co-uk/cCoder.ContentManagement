// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.ComponentModel.DataAnnotations;
using cCoder.ContentManagement.Services.Foundations.Storages;
using cCoder.Data.Models.CMS;
using cCoder.ContentManagement.Models;

namespace cCoder.ContentManagement.Services.Processings;

internal class TemplateProcessingService(ITemplateService service) : ITemplateProcessingService
{
    public Template Get(int id)
    {
        ValidateId(id: id, parameterName: "id");
        return service.Get(id: id);
    }

    public IQueryable<Template> GetAll(bool ignoreFilters = false) =>
        service.GetAll(ignoreFilters: ignoreFilters);

    public ValueTask<Template> AddAsync(Template entity)
    {
        ValidateTemplate(template: entity, parameterName: "entity");
        return service.AddAsync(template: entity);
    }

    public ValueTask<Template> UpdateAsync(Template entity)
    {
        ValidateTemplate(template: entity, parameterName: "entity");
        return service.UpdateAsync(template: entity);
    }

    public ValueTask DeleteAsync(int id)
    {
        ValidateId(id: id, parameterName: "id");
        return service.DeleteAsync(id: id);
    }

    public async ValueTask<IEnumerable<Result<Template>>> AddOrUpdate(IEnumerable<Template> items)
    {
        ValidateTemplates(templates: items, parameterName: "items");
        List<Result<Template>> results = new List<Result<Template>>();

        foreach (Template item in items)
        {
            try
            {
                Template savedItem = item.Id < 1 ? await AddAsync(entity: item) : await UpdateAsync(entity: item);

                results.Add(item: new Result<Template>
                {
                    Success = true,
                    Item = savedItem,
                    Message = item.Id < 1 ? "Added Successfully" : "Updated Successfully"
                });
            }
            catch (Exception ex)
            {
                results.Add(item: new Result<Template>
                {
                    Success = false,
                    Item = item,
                    Message = ex.Message
                });
            }
        }

        return results;
    }

    public async ValueTask DeleteAllAsync(IEnumerable<Template> items)
    {
        ValidateTemplates(templates: items, parameterName: "items");

        foreach (Template item in items)
        {
            await DeleteAsync(id: item.Id);
        }
    }

    private static void ValidateId(int id, string parameterName) =>
        ThrowIf(condition: id < 1, message: parameterName + " must be greater than 0.");

    private static void ValidateTemplate(Template template, string parameterName) =>
        ThrowIf(condition: template == null, message: parameterName + " is required.");

    private static void ValidateTemplates(IEnumerable<Template> templates, string parameterName) =>
        ThrowIf(condition: templates == null, message: parameterName + " is required.");

    private static void ThrowIf(bool condition, string message)
    {
        if (condition)
        {
            throw new ValidationException(message: message);
        }
    }
}