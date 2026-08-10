// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

namespace cCoder.ContentManagement.Services.Foundations.Events;

internal interface IEventHandlerService
{
    void ListenToAllEvents();
    void ListenToWebCacheEvents();
    void ListenToHostedEvents();
    void ListenToFinalAppDeleteEvent();
}