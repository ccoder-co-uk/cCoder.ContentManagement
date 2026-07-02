using cCoder.Data.Models.CMS;

namespace cCoder.ContentManagement.Services.Coordinations;

public interface IAppSupportingResourcesCoordinationService
{
    ValueTask HandleAppAddAsync(App app);

    ValueTask HandleAppDeleteAsync(App app);

    ValueTask HandleAppUpdateAsync(App app);
}
