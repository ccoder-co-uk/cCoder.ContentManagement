using System.ComponentModel.DataAnnotations;
using cCoder.ContentManagement.Services.Foundations.Storages;
using Template = cCoder.Data.Models.CMS.Template;

namespace cCoder.ContentManagement.Services.Processings;

internal class TemplateProcessingService(ITemplateService service) : ITemplateProcessingService
{
    public Template Get(int id)
    {
        ValidateId(id, "id");
        return service.Get(id);
    }

    public IQueryable<Template> GetAll(bool ignoreFilters = false)
    {
        return service.GetAll(ignoreFilters);
    }

    public ValueTask<Template> AddAsync(Template entity)
    {
        ValidateTemplate(entity, "entity");
        return service.AddAsync(entity);
    }

    public ValueTask<Template> UpdateAsync(Template entity)
    {
        ValidateTemplate(entity, "entity");
        return service.UpdateAsync(entity);
    }

    public ValueTask DeleteAsync(int id)
    {
        ValidateId(id, "id");
        return service.DeleteAsync(id);
    }

    public async ValueTask<IEnumerable<cCoder.ContentManagement.Models.Result<Template>>> AddOrUpdate(IEnumerable<Template> items)
    {
        ValidateTemplates(items, "items");
        List<cCoder.ContentManagement.Models.Result<Template>> results = new List<cCoder.ContentManagement.Models.Result<Template>>();
        foreach (Template item in items)
        {
            try
            {
                Template savedItem = item.Id < 1 ? await AddAsync(item) : await UpdateAsync(item);
                results.Add(new cCoder.ContentManagement.Models.Result<Template>
                {
                    Success = true,
                    Item = savedItem,
                    Message = item.Id < 1 ? "Added Successfully" : "Updated Successfully"
                });
            }
            catch (Exception ex)
            {
                results.Add(new cCoder.ContentManagement.Models.Result<Template>
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
        ValidateTemplates(items, "items");
        foreach (Template item in items)
        {
            await DeleteAsync(item.Id);
        }
    }

    private static void ValidateId(int id, string parameterName)
    {
        if (id < 1)
        {
            throw new ValidationException(parameterName + " must be greater than 0.");
        }
    }

    private static void ValidateTemplate(Template template, string parameterName)
    {
        if (template == null)
        {
            throw new ValidationException(parameterName + " is required.");
        }
    }

    private static void ValidateTemplates(IEnumerable<Template> templates, string parameterName)
    {
        if (templates == null)
        {
            throw new ValidationException(parameterName + " is required.");
        }
    }
}

