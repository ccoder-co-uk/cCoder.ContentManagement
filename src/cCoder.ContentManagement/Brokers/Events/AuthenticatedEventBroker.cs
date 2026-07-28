// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Eventing;
using cCoder.Eventing.Models;

namespace cCoder.ContentManagement.Brokers.Events;

public interface IAuthenticatedEventBroker
{
    string GetCurrentUserId();
}

internal abstract class AuthenticatedEventBroker(
    IAuthenticatedEventHub eventHub)
    : IAuthenticatedEventBroker
{
    public string GetCurrentUserId() =>
        eventHub.CurrentUserId;

    protected ValueTask RaiseEventAsync<T>(string name, EventMessage<T> message) =>
        eventHub.RaiseEventAsync(name: name, message: message);
}