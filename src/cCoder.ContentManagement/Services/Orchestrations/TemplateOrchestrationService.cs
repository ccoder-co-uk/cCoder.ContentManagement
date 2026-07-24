// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.ComponentModel.DataAnnotations;
using cCoder.Data.Models.CMS;
using cCoder.ContentManagement.Models;
using System.Security;
using cCoder.ContentManagement.Services.Processings;

namespace cCoder.ContentManagement.Services.Orchestrations;

internal partial class TemplateOrchestrationService(
    ITemplateProcessingService processingService,
    ITemplateEventProcessingService eventService) : ITemplateOrchestrationService
{

    public Template GetTemplate(int templateId) =>
        TryCatch<Template>(operation: () =>
    {
        ValidateTemplateOnGet(inputs: [templateId]);
        return processingService.GetTemplate(templateId: templateId);
    });

    public IQueryable<Template> GetAllTemplate(bool ignoreFilters = false) =>
        TryCatch<IQueryable<Template>>(operation: () =>
    {
        ValidateAllTemplateOnGet(inputs: [ignoreFilters]);
        return processingService.GetAllTemplate(ignoreFilters: ignoreFilters);
    });

    public ValueTask<Template> AddTemplateAsync(Template newTemplate) =>
        TryCatch<Template>(operation: async () =>
    {
        ValidateTemplateOnAdd(inputs: [newTemplate]);
        ValidateTemplate(template: newTemplate, parameterName: "entity");

        Template result = await processingService.AddTemplateAsync(newTemplate: newTemplate);
        await eventService.RaiseTemplateAddEventAsync(entity: result);
        return result;

    }, isValueTask: true);

    public ValueTask<Template> UpdateTemplateAsync(Template updatedTemplate) =>
        TryCatch<Template>(operation: async () =>
    {
        ValidateTemplateOnUpdate(inputs: [updatedTemplate]);
        ValidateTemplate(template: updatedTemplate, parameterName: "entity");

        Template result = await processingService.UpdateTemplateAsync(updatedTemplate: updatedTemplate);
        await eventService.RaiseTemplateUpdateEventAsync(entity: result);
        return result;

    }, isValueTask: true);

    public ValueTask DeleteAsync(int templateId) =>
        TryCatch(operation: async () =>
    {
        ValidateDeleteAsync(inputs: [templateId]);
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

    }, isValueTask: true);

    public ValueTask DeleteByAppIdAsync(int appId) =>
        TryCatch(operation: async () =>
    {
        ValidateByAppIdOnDelete(inputs: [appId]);

        Template[] templatesToDelete = [.. ExecuteGetAllTemplate(ignoreFilters: true)
            .Where(predicate: template => template.AppId == appId)];

        if (templatesToDelete.Length > 0)
        {
            await ExecuteDeleteAllTemplateAsync(deletedTemplate: templatesToDelete);
        }

    }, isValueTask: true);

    public ValueTask<IEnumerable<OperationResult<Template>>> AddOrUpdateTemplateResult(IEnumerable<Template> newTemplate) =>
        TryCatch<IEnumerable<OperationResult<Template>>>(operation: async () =>
    {
        ValidateOrUpdateTemplateResultOnAdd(inputs: [newTemplate]);
        Template[] templates = (newTemplate ?? []).ToArray();
        List<OperationResult<Template>> results = new();

        foreach (Template template in templates)
        {
            try
            {
                Template result = template.Id <= 0
                    ? await ExecuteAddTemplateAsync(newTemplate: template)
                    : await ExecuteUpdateTemplateAsync(updatedTemplate: template);

                results.Add(item: new OperationResult<Template>
                {
                    Success = true,
                    Item = result,
                    Message = template.Id <= 0 ? "Added Successfully" : "Updated Successfully"
                });
            }
            catch (Exception ex)
            {
                results.Add(item: new OperationResult<Template>
                {
                    Success = false,
                    Item = template,
                    Message = ex.Message
                });
            }
        }

        return results;

    }, isValueTask: true);

    public ValueTask ImportTemplatesAsync(int appId, Template[] items) =>
        TryCatch(operation: async () =>
    {
        ValidateImportTemplatesAsync(inputs: [appId, items]);
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

        await ExecuteAddOrUpdateTemplateResult(newTemplate: validatedItems);

    }, isValueTask: true);

    public ValueTask DeleteAllTemplateAsync(IEnumerable<Template> deletedTemplate) =>
        TryCatch(operation: async () =>
    {
        ValidateAllTemplateOnDelete(inputs: [deletedTemplate]);
        Template[] templates = (deletedTemplate ?? []).ToArray();

        foreach (Template template in templates)
        {
            await ExecuteDeleteAsync(templateId: template.Id);
        }

    }, isValueTask: true);

    private static Template ValidateTemplate(Template template, string parameterName)
    {
        if (template == null)
        {
            throw new ValidationException(message: parameterName + " is required.");
        }

        return template;
    }

    private async ValueTask<IEnumerable<OperationResult<Template>>> ExecuteAddOrUpdateTemplateResult(IEnumerable<Template> newTemplate)
    {
        Template[] templates = (newTemplate ?? []).ToArray();
        List<OperationResult<Template>> results = new();

        foreach (Template template in templates)
        {
            try
            {
                Template result = template.Id <= 0
                    ? await ExecuteAddTemplateAsync(newTemplate: template)
                    : await ExecuteUpdateTemplateAsync(updatedTemplate: template);

                results.Add(item: new OperationResult<Template>
                {
                    Success = true,
                    Item = result,
                    Message = template.Id <= 0 ? "Added Successfully" : "Updated Successfully"
                });
            }
            catch (Exception ex)
            {
                results.Add(item: new OperationResult<Template>
                {
                    Success = false,
                    Item = template,
                    Message = ex.Message
                });
            }
        }

        return results;
    }

    private async ValueTask<Template> ExecuteAddTemplateAsync(Template newTemplate)
    {
        ValidateTemplate(template: newTemplate, parameterName: "entity");

        Template result = await processingService.AddTemplateAsync(newTemplate: newTemplate);
        await eventService.RaiseTemplateAddEventAsync(entity: result);
        return result;
    }

    private async ValueTask ExecuteDeleteAllTemplateAsync(IEnumerable<Template> deletedTemplate)
    {
        Template[] templates = (deletedTemplate ?? []).ToArray();

        foreach (Template template in templates)
        {
            await ExecuteDeleteAsync(templateId: template.Id);
        }
    }

    private async ValueTask ExecuteDeleteAsync(int templateId)
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

    private IQueryable<Template> ExecuteGetAllTemplate(bool ignoreFilters = false) =>
        processingService.GetAllTemplate(ignoreFilters: ignoreFilters);

    private async ValueTask<Template> ExecuteUpdateTemplateAsync(Template updatedTemplate)
    {
        ValidateTemplate(template: updatedTemplate, parameterName: "entity");

        Template result = await processingService.UpdateTemplateAsync(updatedTemplate: updatedTemplate);
        await eventService.RaiseTemplateUpdateEventAsync(entity: result);
        return result;
    }
}