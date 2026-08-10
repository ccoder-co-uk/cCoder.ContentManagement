// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

namespace cCoder.ContentManagement.Exposures.EventHandlers;

public interface IContentManagementEventHandlers
{
    void ListenToAllEvents();
    void ListenToWebCacheEvents();
    void ListenToHostedEvents();
    void ListenToFinalAppDeleteEvent();
}