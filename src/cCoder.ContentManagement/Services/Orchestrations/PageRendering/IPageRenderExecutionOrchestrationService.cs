using cCoder.ContentManagement.Rendering.Models;

namespace cCoder.ContentManagement.Rendering.Services.Orchestrations;

internal interface IPageRenderExecutionOrchestrationService
{
    PageRenderResult Render(PageRenderSession session);
}
