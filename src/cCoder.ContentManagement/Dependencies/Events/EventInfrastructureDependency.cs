// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data;
using cCoder.Eventing;
using cCoder.Eventing.Models;

namespace cCoder.ContentManagement.Dependencies.Events;

internal interface IEventInfrastructureDependency
{
    string GetCurrentUserId();

    ValueTask RaiseEventAsync<T>(string name, EventMessage<T> message);
}

internal sealed class EventInfrastructureDependency(
    IEventHub eventHub,
    ICoreAuthInfo authInfo) : IEventInfrastructureDependency
{
    public string GetCurrentUserId() =>
        authInfo.SSOUserId;

    public ValueTask RaiseEventAsync<T>(string name, EventMessage<T> message) =>
        eventHub.RaiseEventAsync(name: name, message: message);
}
