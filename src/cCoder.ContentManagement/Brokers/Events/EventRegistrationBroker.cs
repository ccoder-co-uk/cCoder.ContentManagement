// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Eventing;

namespace cCoder.ContentManagement.Brokers.Events;

internal sealed class EventRegistrationBroker(IEventHub eventHub)
    : IEventRegistrationBroker
{
    public void ListenToEvent<T, TService>(
        string name,
        Func<TService, T, ValueTask> handler) =>
        eventHub.ListenToEvent(name: name, handler: handler);
}