// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Services.Foundations.ServiceProviders;
using cCoder.ContentManagement.Services.Orchestrations;
using cCoder.Data.Models.CMS;

namespace cCoder.ContentManagement.Exposures;

internal sealed class TemplateManager(
    IServiceProviderExecutionService serviceProviderExecutionService)
        : ITemplateManager
{
    public string Render(int appId, string name, string culture, dynamic model) =>
        serviceProviderExecutionService.Execute<
            ITemplateRenderOrchestrationService,
            string>(
                name: "TemplateRender",
                operation: service => service.Render(
                    appId: appId,
                    name: name,
                    culture: culture,
                    model: model));

    public IQueryable<Template> GetAll() =>
        serviceProviderExecutionService.Execute<
            ITemplateOrchestrationService,
            IQueryable<Template>>(
                name: "Template",
                operation: service => service.GetAllTemplate());

    public Template Get(int templateId) =>
        serviceProviderExecutionService.Execute<
            ITemplateOrchestrationService,
            Template>(
                name: "Template",
                operation: service => service.GetTemplate(
                    templateId: templateId));

    public ValueTask<Template> AddAsync(Template newTemplate) =>
        serviceProviderExecutionService.Execute<
            ITemplateOrchestrationService,
            ValueTask<Template>>(
                name: "Template",
                operation: service => service.AddTemplateAsync(
                    newTemplate: newTemplate));

    public ValueTask<Template> UpdateAsync(Template updatedTemplate) =>
        serviceProviderExecutionService.Execute<
            ITemplateOrchestrationService,
            ValueTask<Template>>(
                name: "Template",
                operation: service => service.UpdateTemplateAsync(
                    updatedTemplate: updatedTemplate));

    public ValueTask DeleteAsync(int templateId) =>
        serviceProviderExecutionService.Execute<
            ITemplateOrchestrationService,
            ValueTask>(
                name: "Template",
                operation: service => service.DeleteAsync(
                    templateId: templateId));
}