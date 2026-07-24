// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

namespace cCoder.ContentManagement.Brokers.ServiceProviders;

internal interface IServiceProviderBroker
{
    TService GetRequiredService<TService>(string name)
        where TService : notnull;
}