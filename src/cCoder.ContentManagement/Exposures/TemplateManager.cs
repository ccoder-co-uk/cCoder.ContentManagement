// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Services.Foundations.ServiceProviders;
using cCoder.ContentManagement.Services.Orchestrations;
using cCoder.ContentManagement.Brokers;
using cCoder.Data.Models.CMS;

namespace cCoder.ContentManagement.Exposures;

internal sealed class TemplateManager(
    IServiceProviderExecutionService serviceProviderExecutionService)
        : ITemplateManager
{
    public ValueTask<string> ReadContentAsync(Stream source) =>
        serviceProviderExecutionService.Execute<
            ITemplateContentBroker,
            ValueTask<string>>(
                name: "TemplateContent",
                operation: broker => broker.ReadAsync(source: source));

    public byte[] ConvertHtmlToPdf(string html) =>
        serviceProviderExecutionService.Execute<
            ITemplateContentBroker,
            byte[]>(
                name: "TemplateContent",
                operation: broker => broker.ConvertHtmlToPdf(html: html));

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

    public ValueTask ImportTemplatesAsync(int appId, Template[] items) =>
        serviceProviderExecutionService.Execute<
            ITemplateOrchestrationService,
            ValueTask>(
                name: "Template",
                operation: service => service.ImportTemplatesAsync(
                    appId: appId,
                    items: items));
}