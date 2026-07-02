using cCoder.Data.Models.CMS;

namespace cCoder.ContentManagement.Services.Coordinations;

public interface IPageCoordinationService
{
    ValueTask HandlePageAddAsync(Page page);

    ValueTask HandlePageUpdateAsync(Page page);

    ValueTask HandlePageDeleteAsync(Page page);
}
