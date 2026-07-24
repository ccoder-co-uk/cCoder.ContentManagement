// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

namespace cCoder.ContentManagement.Services.Foundations.ServiceProviders;

internal interface IServiceProviderExecutionService
{
    TResult Execute<TService, TResult>(
        string name,
        Func<TService, TResult> operation)
        where TService : notnull;
}
