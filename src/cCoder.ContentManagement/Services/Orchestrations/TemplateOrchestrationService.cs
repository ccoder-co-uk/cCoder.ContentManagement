using System.ComponentModel.DataAnnotations;
using System.Security;
using cCoder.ContentManagement.Services.Processings;
using Template = cCoder.Data.Models.CMS.Template;
using Result = cCoder.ContentManagement.Models.Result<cCoder.Data.Models.CMS.Template>;

namespace cCoder.ContentManagement.Services.Orchestrations;

internal class TemplateOrchestrationService(
    ITemplateProcessingService processingService,
    ITemplateEventProcessingService eventService) : ITemplateOrchestrationService
{

    public Template Get(int id) => processingService.Get(id);

    public IQueryable<Template> GetAll(bool ignoreFilters = false) =>
        processingService.GetAll(ignoreFilters);

    public async ValueTask<Template> AddAsync(Template entity)
    {
        ValidateTemplate(entity, "entity");

        Template result = await processingService.AddAsync(entity);
        await eventService.RaiseTemplateAddEventAsync(result);
        return result;
    }

    public async ValueTask<Template> UpdateAsync(Template entity)
    {
        ValidateTemplate(entity, "entity");

        Template result = await processingService.UpdateAsync(entity);
        await eventService.RaiseTemplateUpdateEventAsync(result);
        return result;
    }

    public async ValueTask DeleteAsync(int id)
    {
        Template entity;
        try
        {
            entity = processingService.Get(id);
        }
        catch (SecurityException)
        {
            entity = processingService.GetAll(ignoreFilters: true)
                .FirstOrDefault(template => template.Id == id);
        }

        if (entity == null)
        {
            return;
        }

        await eventService.RaiseTemplateDeleteEventAsync(entity);
        await processingService.DeleteAsync(id);
    }

    public async ValueTask DeleteByAppIdAsync(int appId)
    {
        Template[] templatesToDelete = [.. GetAll(ignoreFilters: true).Where(template => template.AppId == appId)];

        if (templatesToDelete.Length > 0)
        {
            await DeleteAllAsync(templatesToDelete);
        }
    }

    public async ValueTask<IEnumerable<Result>> AddOrUpdate(IEnumerable<Template> items)
    {
        Template[] templates = (items ?? []).ToArray();
        List<Result> results = new();

        foreach (Template template in templates)
        {
            try
            {
                Template result = template.Id <= 0
                    ? await AddAsync(template)
                    : await UpdateAsync(template);

                results.Add(new Result
                {
                    Success = true,
                    Item = result,
                    Message = template.Id <= 0 ? "Added Successfully" : "Updated Successfully"
                });
            }
            catch (Exception ex)
            {
                results.Add(new Result
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
        string[] names = validatedItems.Select(template => template.Name.ToLower()).ToArray();

        var dbVersions = (from template in processingService.GetAll()
                          where template.AppId == appId && ((ReadOnlySpan<string>)names).Contains(template.Name.ToLower())
                          select new { template.Id, template.Name }).ToArray();

        Array.ForEach(validatedItems, template =>
        {
            template.AppId = appId;
            template.Id = dbVersions.FirstOrDefault(existing => existing.Name == template.Name)?.Id ?? 0;
        });

        await AddOrUpdate(validatedItems);
    }

    public async ValueTask DeleteAllAsync(IEnumerable<Template> items)
    {
        Template[] templates = (items ?? []).ToArray();

        foreach (Template template in templates)
        {
            await DeleteAsync(template.Id);
        }
    }

    private static Template ValidateTemplate(Template template, string parameterName)
    {
        if (template == null)
        {
            throw new ValidationException(parameterName + " is required.");
        }

        return template;
    }
}
