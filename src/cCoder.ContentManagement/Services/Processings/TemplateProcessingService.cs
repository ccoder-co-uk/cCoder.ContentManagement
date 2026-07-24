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
    public Template GetTemplate(int templateId)
    {
        ValidateId(templateId: templateId, parameterName: "id");
        return service.GetTemplate(templateId: templateId);
    }

    public IQueryable<Template> GetAllTemplate(bool ignoreFilters = false) =>
        service.GetAllTemplate(ignoreFilters: ignoreFilters);

    public ValueTask<Template> AddTemplateAsync(Template newTemplate)
    {
        ValidateTemplate(template: newTemplate, parameterName: "entity");
        return service.AddTemplateAsync(newTemplate: newTemplate);
    }

    public ValueTask<Template> UpdateTemplateAsync(Template updatedTemplate)
    {
        ValidateTemplate(template: updatedTemplate, parameterName: "entity");
        return service.UpdateTemplateAsync(updatedTemplate: updatedTemplate);
    }

    public ValueTask DeleteAsync(int templateId)
    {
        ValidateId(templateId: templateId, parameterName: "id");
        return service.DeleteAsync(templateId: templateId);
    }

    public async ValueTask<IEnumerable<Result<Template>>> AddOrUpdateTemplateResult(IEnumerable<Template> newTemplate)
    {
        ValidateTemplates(templates: newTemplate, parameterName: "items");
        List<Result<Template>> results = new List<Result<Template>>();

        foreach (Template item in newTemplate)
        {
            try
            {
                Template savedItem = item.Id < 1 ? await ExecuteAddTemplateAsync(newTemplate: item) : await ExecuteUpdateTemplateAsync(updatedTemplate: item);

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

    public async ValueTask DeleteAllTemplateAsync(IEnumerable<Template> deletedTemplate)
    {
        ValidateTemplates(templates: deletedTemplate, parameterName: "items");

        foreach (Template item in deletedTemplate)
        {
            await ExecuteDeleteAsync(templateId: item.Id);
        }
    }

    private static void ValidateId(int templateId, string parameterName) =>
        ThrowIf(condition: templateId < 1, message: parameterName + " must be greater than 0.");

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

    private ValueTask<Template> ExecuteAddTemplateAsync(Template newTemplate)
    {
        ValidateTemplate(template: newTemplate, parameterName: "entity");
        return service.AddTemplateAsync(newTemplate: newTemplate);
    }

    private ValueTask ExecuteDeleteAsync(int templateId)
    {
        ValidateId(templateId: templateId, parameterName: "id");
        return service.DeleteAsync(templateId: templateId);
    }

    private ValueTask<Template> ExecuteUpdateTemplateAsync(Template updatedTemplate)
    {
        ValidateTemplate(template: updatedTemplate, parameterName: "entity");
        return service.UpdateTemplateAsync(updatedTemplate: updatedTemplate);
    }
}