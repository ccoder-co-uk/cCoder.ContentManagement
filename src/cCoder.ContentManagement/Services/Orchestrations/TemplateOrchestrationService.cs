// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.ComponentModel.DataAnnotations;
using cCoder.Data.Models.CMS;
using cCoder.ContentManagement.Models;
using System.Security;
using cCoder.ContentManagement.Services.Processings;

namespace cCoder.ContentManagement.Services.Orchestrations;

internal class TemplateOrchestrationService(
    ITemplateProcessingService processingService,
    ITemplateEventProcessingService eventService) : ITemplateOrchestrationService
{

    public Template GetTemplate(int templateId) =>
        processingService.GetTemplate(templateId: templateId);

    public IQueryable<Template> GetAllTemplate(bool ignoreFilters = false) =>
        processingService.GetAllTemplate(ignoreFilters: ignoreFilters);

    public async ValueTask<Template> AddTemplateAsync(Template newTemplate)
    {
        ValidateTemplate(template: newTemplate, parameterName: "entity");

        Template result = await processingService.AddTemplateAsync(newTemplate: newTemplate);
        await eventService.RaiseTemplateAddEventAsync(entity: result);
        return result;
    }

    public async ValueTask<Template> UpdateTemplateAsync(Template updatedTemplate)
    {
        ValidateTemplate(template: updatedTemplate, parameterName: "entity");

        Template result = await processingService.UpdateTemplateAsync(updatedTemplate: updatedTemplate);
        await eventService.RaiseTemplateUpdateEventAsync(entity: result);
        return result;
    }

    public async ValueTask DeleteAsync(int templateId)
    {
        Template entity;

        try
        {
            entity = processingService.GetTemplate(templateId: templateId);
        }
        catch (SecurityException)
        {
            entity = processingService.GetAllTemplate(ignoreFilters: true)
                .FirstOrDefault(predicate: template => template.Id == templateId);
        }

        if (entity == null)
        {
            return;
        }

        await eventService.RaiseTemplateDeleteEventAsync(entity: entity);
        await processingService.DeleteAsync(templateId: templateId);
    }

    public async ValueTask DeleteByAppIdAsync(int appId)
    {
        Template[] templatesToDelete = [.. GetAllTemplate(ignoreFilters: true)
            .Where(predicate: template => template.AppId == appId)];

        if (templatesToDelete.Length > 0)
        {
            await DeleteAllTemplateAsync(deletedTemplate: templatesToDelete);
        }
    }

    public async ValueTask<IEnumerable<Result<Template>>> AddOrUpdateTemplateResult(IEnumerable<Template> newTemplate)
    {
        Template[] templates = (newTemplate ?? []).ToArray();
        List<Result<Template>> results = new();

        foreach (Template template in templates)
        {
            try
            {
                Template result = template.Id <= 0
                    ? await AddTemplateAsync(newTemplate: template)
                    : await UpdateTemplateAsync(updatedTemplate: template);

                results.Add(item: new Result<Template>
                {
                    Success = true,
                    Item = result,
                    Message = template.Id <= 0 ? "Added Successfully" : "Updated Successfully"
                });
            }
            catch (Exception ex)
            {
                results.Add(item: new Result<Template>
                {
                    Success = false,
                    Item = template,
                    Message = ex.Message
                });
            }
        }

        return results;
    }

    public async ValueTask ImportTemplatesAsync(int appId, Template[] items)
    {
        Template[] validatedItems = items ?? [];

        string[] names = validatedItems.Select(selector: template => template.Name.ToLower())
            .ToArray();

        var dbVersions = processingService.GetAllTemplate()
            .Where(predicate: template => template.AppId == appId && ((ReadOnlySpan<string>)names).Contains(value: template.Name.ToLower()))
            .Select(selector: template => new { template.Id, template.Name })
            .ToArray();

        Array.ForEach(array: validatedItems, action: template =>
        {
            template.AppId = appId;
            template.Id = dbVersions.FirstOrDefault(predicate: existing => existing.Name == template.Name)?.Id ?? 0;
        });

        await AddOrUpdateTemplateResult(newTemplate: validatedItems);
    }

    public async ValueTask DeleteAllTemplateAsync(IEnumerable<Template> deletedTemplate)
    {
        Template[] templates = (deletedTemplate ?? []).ToArray();

        foreach (Template template in templates)
        {
            await DeleteAsync(templateId: template.Id);
        }
    }

    private static Template ValidateTemplate(Template template, string parameterName)
    {
        if (template == null)
        {
            throw new ValidationException(message: parameterName + " is required.");
        }

        return template;
    }
}