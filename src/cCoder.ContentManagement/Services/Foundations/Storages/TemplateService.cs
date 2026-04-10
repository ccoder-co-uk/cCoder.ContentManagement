using System.Security;
using cCoder.ContentManagement.Brokers;
using cCoder.ContentManagement.Brokers.Storages;
using Template = cCoder.Data.Models.CMS.Template;

namespace cCoder.ContentManagement.Services.Foundations.Storages;

internal partial class TemplateService(ITemplateBroker templateBroker, IAuthorizationBroker authorizationBroker) : ITemplateService
{
    public Template Get(int id, bool ignoreFilters = false)
    {
        ValidateId(id, "id");
        if (ignoreFilters)
        {
            return GetAll(ignoreFilters: true).FirstOrDefault((Template i) => i.Id == id);
        }

        Template template = GetAll().FirstOrDefault((Template i) => i.Id == id);
        if (template != null)
        {
            return template;
        }
        Template template2 = GetAll(ignoreFilters: true).FirstOrDefault((Template i) => i.Id == id);
        if (template2 != null)
        {
            throw new SecurityException("Access Denied!");
        }
        return null;
    }

    public IQueryable<Template> GetAll(bool ignoreFilters = false)
    {
        return templateBroker.GetAllTemplates(ignoreFilters);
    }

    public async ValueTask<Template> AddAsync(Template template)
    {
        ValidateTemplate(template, "template");
        authorizationBroker.Authorize(template.AppId, "Template_create");
        Template newTemplate = CreateStorageTemplate(template);
        string currentUserId = authorizationBroker.GetCurrentUser().Id;
        DateTimeOffset now = (newTemplate.CreatedOn = DateTimeOffset.UtcNow);
        newTemplate.CreatedBy = currentUserId;
        newTemplate.LastUpdated = now;
        newTemplate.LastUpdatedBy = currentUserId;
        Template result = await templateBroker.AddTemplateAsync(newTemplate);
        result.App = template.App;
        return result;
    }

    public async ValueTask<Template> UpdateAsync(Template template)
    {
        ValidateTemplate(template, "template");
        authorizationBroker.Authorize(template.AppId, "Template_update");
        Template updateTemplate = CreateStorageTemplate(template);
        string currentUserId = authorizationBroker.GetCurrentUser().Id;
        DateTimeOffset now = DateTimeOffset.UtcNow;
        updateTemplate.LastUpdated = now;
        updateTemplate.LastUpdatedBy = currentUserId;
        Template result = await templateBroker.UpdateTemplateAsync(updateTemplate);
        result.App = template.App;
        return result;
    }

    public async ValueTask DeleteAsync(int id)
    {
        ValidateId(id, "id");
        Template template;
        try
        {
            template = Get(id);
        }
        catch (SecurityException)
        {
            template = Get(id, ignoreFilters: true);
        }

        if (template == null)
        {
            return;
        }

        authorizationBroker.Authorize(template.AppId, "Template_delete");
        await templateBroker.DeleteTemplateAsync(CreateStorageTemplate(template));
    }

    private static Template CreateStorageTemplate(Template template)
    {
        if (template == null)
        {
            return null;
        }

        return new Template
        {
            Id = template.Id,
            Name = template.Name,
            Description = template.Description,
            LastUpdated = template.LastUpdated,
            LastUpdatedBy = template.LastUpdatedBy,
            CreatedOn = template.CreatedOn,
            CreatedBy = template.CreatedBy,
            ResourceKey = template.ResourceKey,
            RawString = template.RawString,
            AppId = template.AppId
        };
    }
}
