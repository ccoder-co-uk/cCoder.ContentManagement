// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data;
using cCoder.Eventing;
using cCoder.Eventing.Models;

namespace cCoder.ContentManagement.Dependencies.Events;

internal sealed class AuthenticatedEventHubDependency(
        IEventHub eventHub,
        ICoreAuthInfo authInfo)
            : IAuthenticatedEventHub
{
    public string CurrentUserId =>
        authInfo.SSOUserId;

    public void ListenToEvent<T, TService>(
        string name,
        Func<TService, T, ValueTask> handler) =>
        eventHub.ListenToEvent(name: name, handler: handler);

    public ValueTask RaiseEventAsync<T>(
        string name,
        EventMessage<T> message) =>
        eventHub.RaiseEventAsync(name: name, message: message);

    public ValueTask RaiseEventsAsync<T>(
        string name,
        EventMessage<T>[] messages) =>
        eventHub.RaiseEventsAsync(name: name, messages: messages);
}