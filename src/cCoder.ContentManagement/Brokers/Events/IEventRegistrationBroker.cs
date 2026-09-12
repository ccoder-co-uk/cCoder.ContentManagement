// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

namespace cCoder.ContentManagement.Brokers.Events;

internal interface IEventRegistrationBroker
{
    void ListenToEvent<T, TService>(
        string name,
        Func<TService, T, ValueTask> handler);
}