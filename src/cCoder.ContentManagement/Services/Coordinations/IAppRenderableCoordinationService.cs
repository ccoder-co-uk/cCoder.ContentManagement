using App = cCoder.Data.Models.CMS.App;

namespace cCoder.ContentManagement.Services.Coordinations;

public interface IAppRenderableCoordinationService
{
    ValueTask HandleAppAddAsync(App app);

    ValueTask HandleAppDeleteAsync(App app);

    ValueTask HandleAppUpdateAsync(App app);
}
