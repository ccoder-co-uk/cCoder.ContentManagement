// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Dependencies.Events;

namespace cCoder.ContentManagement.Brokers.Events;

public interface IAuthenticatedEventBroker
{
    string GetCurrentUserId();
}

internal abstract class AuthenticatedEventBroker(
    IEventInfrastructureDependency eventInfrastructureDependency)
    : IAuthenticatedEventBroker
{
    public string GetCurrentUserId() =>
        eventInfrastructureDependency.GetCurrentUserId();
}
