namespace cCoder.ContentManagement.Exposures;

public interface IPageRenderer
{
    PageRenderResponse Render(PageRenderRequest request);

    PageRenderResponse RenderError(PageRenderRequest request);
}
