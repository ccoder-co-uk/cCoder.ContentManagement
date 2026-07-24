// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Dependencies.Events;
using cCoder.Eventing.Models;

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

    protected ValueTask RaiseEventAsync<T>(string name, EventMessage<T> message) =>
        eventInfrastructureDependency.RaiseEventAsync(name: name, message: message);
}