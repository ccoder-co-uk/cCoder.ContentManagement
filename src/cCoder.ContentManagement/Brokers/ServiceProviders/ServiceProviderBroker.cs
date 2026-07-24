// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using Microsoft.Extensions.DependencyInjection;

namespace cCoder.ContentManagement.Brokers.ServiceProviders;

internal sealed class ServiceProviderBroker(
    IServiceProvider serviceProvider) : IServiceProviderBroker
{
    public TService GetRequiredService<TService>(string name)
        where TService : notnull =>
        serviceProvider.GetRequiredKeyedService<TService>(
            serviceKey: name);
}
