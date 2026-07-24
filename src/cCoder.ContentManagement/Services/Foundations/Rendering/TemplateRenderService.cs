// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Brokers.ServiceProviders;

namespace cCoder.ContentManagement.Services.Foundations.Rendering;

internal sealed partial class TemplateRenderService(
    IServiceProviderBroker serviceProviderBroker) : ITemplateRenderService
{
    public TResult Execute<TService, TResult>(
        string name,
        Func<TService, TResult> operation)
        where TService : notnull =>
        TryCatch(operation: () =>
    {
        ValidateExecute(inputs: [name, operation]);
        ValidateName(name: name);
        ValidateOperation(operation: operation);

        TService service =
            serviceProviderBroker.GetRequiredService<TService>(
                name: name);

        return operation(arg: service);
    });
}