// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

namespace cCoder.ContentManagement.Exposures.EventHandlers;

public interface IContentManagementEventHandlers
{
    void ListenToAllEvents();
    void ListenToHostedEvents();
    void ListenToFinalAppDeleteEvent();
}