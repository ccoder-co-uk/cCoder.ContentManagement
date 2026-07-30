// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Security;
using cCoder.ContentManagement.Brokers;
using cCoder.ContentManagement.Brokers.Storages;
using cCoder.Data.Models.CMS;

using cCoder.ContentManagement.Exposures;

namespace cCoder.ContentManagement.Services.Foundations.Storages;

internal partial class TemplateService(ITemplateBroker templateBroker, IAuthorizationManager authorizationManager) : ITemplateService
{
    public Template GetTemplate(int templateId, bool ignoreFilters = false) =>
        TryCatch<Template>(operation: () =>
    {
        ValidateTemplateOnGet(inputs: [templateId, ignoreFilters]);
        ValidateId(templateId: templateId, parameterName: "id");

        if (ignoreFilters)
        {
            return ExecuteGetAllTemplate(ignoreFilters: true)
                .FirstOrDefault(predicate: (Template i) => i.Id == templateId);
        }

        Template template = ExecuteGetAllTemplate()
            .FirstOrDefault(predicate: (Template i) => i.Id == templateId);

        if (template != null)
        {
            return template;
        }

        Template template2 = ExecuteGetAllTemplate(ignoreFilters: true)
            .FirstOrDefault(predicate: (Template i) => i.Id == templateId);

        if (template2 != null)
        {
            throw new SecurityException(message: "Access Denied!");
        }

        return null;

    });

    public IQueryable<Template> GetAllTemplate(bool ignoreFilters = false) =>
        TryCatch<IQueryable<Template>>(operation: () =>
    {
        ValidateAllTemplateOnGet(inputs: [ignoreFilters]);

        return ignoreFilters
            ? templateBroker.GetAllTemplatesIgnoringFilters()
            : templateBroker.GetAllTemplates();
    });

    public ValueTask<Template> AddTemplateAsync(Template newTemplate) =>
        TryCatch<Template>(operation: async () =>
    {
        ValidateTemplateOnAdd(inputs: [newTemplate]);
        ValidateTemplate(template: newTemplate, parameterName: "template");
        authorizationManager.Authorize(appId: newTemplate.AppId, privilege: "Template_create");
        Template storageTemplate = CreateStorageTemplate(newTemplate: newTemplate);

        string currentUserId = authorizationManager.GetCurrentUser()
            .Id;

        DateTimeOffset now = (storageTemplate.CreatedOn = DateTimeOffset.UtcNow);
        storageTemplate.CreatedBy = currentUserId;
        storageTemplate.LastUpdated = now;
        storageTemplate.LastUpdatedBy = currentUserId;
        Template result = await templateBroker.AddTemplateAsync(newTemplate: storageTemplate);
        newTemplate.Id = result.Id;
        newTemplate.Name = result.Name;
        newTemplate.Description = result.Description;
        newTemplate.LastUpdated = result.LastUpdated;
        newTemplate.LastUpdatedBy = result.LastUpdatedBy;
        newTemplate.CreatedOn = result.CreatedOn;
        newTemplate.CreatedBy = result.CreatedBy;
        newTemplate.AppId = result.AppId;
        newTemplate.ResourceKey = result.ResourceKey;
        newTemplate.RawString = result.RawString;
        return newTemplate;

    }, isValueTask: true);

    public ValueTask<Template> UpdateTemplateAsync(Template updatedTemplate) =>
        TryCatch<Template>(operation: async () =>
    {
        ValidateTemplateOnUpdate(inputs: [updatedTemplate]);
        ValidateTemplate(template: updatedTemplate, parameterName: "template");
        authorizationManager.Authorize(appId: updatedTemplate.AppId, privilege: "Template_update");
        Template updateTemplate = CreateStorageTemplate(newTemplate: updatedTemplate);

        string currentUserId = authorizationManager.GetCurrentUser()
            .Id;

        DateTimeOffset now = DateTimeOffset.UtcNow;
        updateTemplate.LastUpdated = now;
        updateTemplate.LastUpdatedBy = currentUserId;
        Template result = await templateBroker.UpdateTemplateAsync(updatedTemplate: updateTemplate);
        updatedTemplate.Id = result.Id;
        updatedTemplate.Name = result.Name;
        updatedTemplate.Description = result.Description;
        updatedTemplate.LastUpdated = result.LastUpdated;
        updatedTemplate.LastUpdatedBy = result.LastUpdatedBy;
        updatedTemplate.CreatedOn = result.CreatedOn;
        updatedTemplate.CreatedBy = result.CreatedBy;
        updatedTemplate.AppId = result.AppId;
        updatedTemplate.ResourceKey = result.ResourceKey;
        updatedTemplate.RawString = result.RawString;
        return updatedTemplate;

    }, isValueTask: true);

    public ValueTask DeleteAsync(int templateId) =>
        TryCatch(operation: async () =>
    {
        ValidateDeleteAsync(inputs: [templateId]);
        ValidateId(templateId: templateId, parameterName: "id");
        Template template;

        try
        {
            template = ExecuteGetTemplate(templateId: templateId);
        }
        catch (SecurityException)
        {
            template = ExecuteGetTemplate(templateId: templateId, ignoreFilters: true);
        }

        if (template == null)
        {
            return;
        }

        authorizationManager.Authorize(appId: template.AppId, privilege: "Template_delete");
        await templateBroker.DeleteTemplateAsync(deletedTemplate: CreateStorageTemplate(newTemplate: template));

    }, isValueTask: true);

    private static Template CreateStorageTemplate(Template newTemplate)
    {
        if (newTemplate == null)
        {
            return null;
        }

        return new Template
        {
            Id = newTemplate.Id,
            Name = newTemplate.Name,
            Description = newTemplate.Description,
            LastUpdated = newTemplate.LastUpdated,
            LastUpdatedBy = newTemplate.LastUpdatedBy,
            CreatedOn = newTemplate.CreatedOn,
            CreatedBy = newTemplate.CreatedBy,
            ResourceKey = newTemplate.ResourceKey,
            RawString = newTemplate.RawString,
            AppId = newTemplate.AppId
        };
    }

    private IQueryable<Template> ExecuteGetAllTemplate(bool ignoreFilters = false) =>
        (ignoreFilters
            ? templateBroker.GetAllTemplatesIgnoringFilters()
            : templateBroker.GetAllTemplates());

    private Template ExecuteGetTemplate(int templateId, bool ignoreFilters = false)
    {
        ValidateId(templateId: templateId, parameterName: "id");

        if (ignoreFilters)
        {
            return ExecuteGetAllTemplate(ignoreFilters: true)
                .FirstOrDefault(predicate: (Template i) => i.Id == templateId);
        }

        Template template = ExecuteGetAllTemplate()
            .FirstOrDefault(predicate: (Template i) => i.Id == templateId);

        if (template != null)
        {
            return template;
        }

        Template template2 = ExecuteGetAllTemplate(ignoreFilters: true)
            .FirstOrDefault(predicate: (Template i) => i.Id == templateId);

        if (template2 != null)
        {
            throw new SecurityException(message: "Access Denied!");
        }

        return null;
    }
}