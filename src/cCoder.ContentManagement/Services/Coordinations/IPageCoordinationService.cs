using Page = cCoder.Data.Models.CMS.Page;

namespace cCoder.ContentManagement.Services.Coordinations;

public interface IPageCoordinationService
{
    ValueTask HandlePageAddAsync(Page page);

    ValueTask HandlePageUpdateAsync(Page page);

    ValueTask HandlePageDeleteAsync(Page page);
}
