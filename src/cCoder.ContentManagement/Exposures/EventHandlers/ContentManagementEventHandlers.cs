// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Services.Foundations.Events;

namespace cCoder.ContentManagement.Exposures.EventHandlers;

internal class ContentManagementEventHandlers(IEventHandlerService eventHandlerService) : IContentManagementEventHandlers
{
    public void ListenToAllEvents() =>
        eventHandlerService.ListenToAllEvents();

    public void ListenToWebCacheEvents() =>
        eventHandlerService.ListenToWebCacheEvents();

    public void ListenToHostedEvents() =>
        eventHandlerService.ListenToHostedEvents();

    public void ListenToFinalAppDeleteEvent() =>
        eventHandlerService.ListenToFinalAppDeleteEvent();

}