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

    public Template Get(int id) =>
        processingService.Get(id: id);

    public IQueryable<Template> GetAll(bool ignoreFilters = false) =>
        processingService.GetAll(ignoreFilters: ignoreFilters);

    public async ValueTask<Template> AddAsync(Template entity)
    {
        ValidateTemplate(template: entity, parameterName: "entity");

        Template result = await processingService.AddAsync(entity: entity);
        await eventService.RaiseTemplateAddEventAsync(entity: result);
        return result;
    }

    public async ValueTask<Template> UpdateAsync(Template entity)
    {
        ValidateTemplate(template: entity, parameterName: "entity");

        Template result = await processingService.UpdateAsync(entity: entity);
        await eventService.RaiseTemplateUpdateEventAsync(entity: result);
        return result;
    }

    public async ValueTask DeleteAsync(int id)
    {
        Template entity;

        try
        {
            entity = processingService.Get(id: id);
        }
        catch (SecurityException)
        {
            entity = processingService.GetAll(ignoreFilters: true)
                .FirstOrDefault(predicate: template => template.Id == id);
        }

        if (entity == null)
        {
            return;
        }

        await eventService.RaiseTemplateDeleteEventAsync(entity: entity);
        await processingService.DeleteAsync(id: id);
    }

    public async ValueTask DeleteByAppIdAsync(int appId)
    {
        Template[] templatesToDelete = [.. GetAll(ignoreFilters: true)
            .Where(predicate: template => template.AppId == appId)];

        if (templatesToDelete.Length > 0)
        {
            await DeleteAllAsync(items: templatesToDelete);
        }
    }

    public async ValueTask<IEnumerable<Result<Template>>> AddOrUpdate(IEnumerable<Template> items)
    {
        Template[] templates = (items ?? []).ToArray();
        List<Result<Template>> results = new();

        foreach (Template template in templates)
        {
            try
            {
                Template result = template.Id <= 0
                    ? await AddAsync(entity: template)
                    : await UpdateAsync(entity: template);

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

        var dbVersions = processingService.GetAll()
            .Where(predicate: template => template.AppId == appId && ((ReadOnlySpan<string>)names).Contains(value: template.Name.ToLower()))
            .Select(selector: template => new { template.Id, template.Name })
            .ToArray();

        Array.ForEach(array: validatedItems, action: template =>
        {
            template.AppId = appId;
            template.Id = dbVersions.FirstOrDefault(predicate: existing => existing.Name == template.Name)?.Id ?? 0;
        });

        await AddOrUpdate(items: validatedItems);
    }

    public async ValueTask DeleteAllAsync(IEnumerable<Template> items)
    {
        Template[] templates = (items ?? []).ToArray();

        foreach (Template template in templates)
        {
            await DeleteAsync(id: template.Id);
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