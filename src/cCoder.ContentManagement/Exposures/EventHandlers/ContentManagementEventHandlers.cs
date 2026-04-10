using cCoder.ContentManagement.Services.Foundations.Events;

namespace cCoder.ContentManagement.Exposures.EventHandlers;

internal class ContentManagementEventHandlers(IEventHandlerService eventHandlerService) : IContentManagementEventHandlers
{
    public void ListenToAllEvents()
    {
        eventHandlerService.ListenToAllEvents();
    }
}
