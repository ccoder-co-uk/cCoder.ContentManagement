// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Security;
using cCoder.ContentManagement.Brokers;
using cCoder.ContentManagement.Brokers.Storages;
using cCoder.Data.Models.CMS;

namespace cCoder.ContentManagement.Services.Foundations.Storages;

internal partial class TemplateService(ITemplateBroker templateBroker, IAuthorizationBroker authorizationBroker) : ITemplateService
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
        return templateBroker.GetAllTemplates(ignoreFilters: ignoreFilters);
    });

    public ValueTask<Template> AddTemplateAsync(Template template) =>
        TryCatch<Template>(operation: async () =>
    {
        ValidateTemplateOnAdd(inputs: [template]);
        ValidateTemplate(template: template, parameterName: "template");
        authorizationBroker.Authorize(appId: template.AppId, privilege: "Template_create");
        Template newTemplate = CreateStorageTemplate(newTemplate: template);

        string currentUserId = authorizationBroker.GetCurrentUser()
            .Id;

        DateTimeOffset now = (newTemplate.CreatedOn = DateTimeOffset.UtcNow);
        newTemplate.CreatedBy = currentUserId;
        newTemplate.LastUpdated = now;
        newTemplate.LastUpdatedBy = currentUserId;
        Template result = await templateBroker.AddTemplateAsync(newTemplate: newTemplate);
        template.Id = result.Id;
        template.Name = result.Name;
        template.Description = result.Description;
        template.LastUpdated = result.LastUpdated;
        template.LastUpdatedBy = result.LastUpdatedBy;
        template.CreatedOn = result.CreatedOn;
        template.CreatedBy = result.CreatedBy;
        template.AppId = result.AppId;
        template.ResourceKey = result.ResourceKey;
        template.RawString = result.RawString;
        return template;

    }, isValueTask: true);

    public ValueTask<Template> UpdateTemplateAsync(Template updatedTemplate) =>
        TryCatch<Template>(operation: async () =>
    {
        ValidateTemplateOnUpdate(inputs: [updatedTemplate]);
        ValidateTemplate(template: updatedTemplate, parameterName: "template");
        authorizationBroker.Authorize(appId: updatedTemplate.AppId, privilege: "Template_update");
        Template updateTemplate = CreateStorageTemplate(newTemplate: updatedTemplate);

        string currentUserId = authorizationBroker.GetCurrentUser()
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

        authorizationBroker.Authorize(appId: template.AppId, privilege: "Template_delete");
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
        templateBroker.GetAllTemplates(ignoreFilters: ignoreFilters);

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