using System.Security;
using cCoder.ContentManagement.Brokers;
using cCoder.ContentManagement.Brokers.Storages;
using Layout = cCoder.Data.Models.CMS.Layout;

namespace cCoder.ContentManagement.Services.Foundations.Storages;

internal partial class LayoutService(ILayoutBroker layoutBroker, IAuthorizationBroker authorizationBroker) : ILayoutService
{
    public Layout Get(int id, bool ignoreFilters = false)
    {
        ValidateId(id, "id");
        if (ignoreFilters)
        {
            return GetAll(ignoreFilters: true).FirstOrDefault((Layout i) => i.Id == id);
        }

        Layout layout = GetAll().FirstOrDefault((Layout i) => i.Id == id);
        if (layout != null)
        {
            return layout;
        }
        Layout layout2 = GetAll(ignoreFilters: true).FirstOrDefault((Layout i) => i.Id == id);
        if (layout2 != null)
        {
            throw new SecurityException("Access Denied!");
        }
        return null;
    }

    public IQueryable<Layout> GetAll(bool ignoreFilters = false)
    {
        return layoutBroker.GetAllLayouts(ignoreFilters);
    }

    public async ValueTask<Layout> AddAsync(Layout layout)
    {
        ValidateLayout(layout, "layout");
        authorizationBroker.Authorize(layout.AppId, "Layout_create");
        Layout newLayout = CreateStorageLayout(layout);
        string currentUserId = authorizationBroker.GetCurrentUser().Id;
        DateTimeOffset now = (newLayout.CreatedOn = DateTimeOffset.UtcNow);
        newLayout.CreatedBy = currentUserId;
        newLayout.LastUpdated = now;
        newLayout.LastUpdatedBy = currentUserId;
        Layout result = await layoutBroker.AddLayoutAsync(newLayout);
        result.App = layout.App;
        return result;
    }

    public async ValueTask<Layout> UpdateAsync(Layout layout)
    {
        ValidateLayout(layout, "layout");
        authorizationBroker.Authorize(layout.AppId, "Layout_update");
        Layout updateLayout = CreateStorageLayout(layout);
        string currentUserId = authorizationBroker.GetCurrentUser().Id;
        DateTimeOffset now = DateTimeOffset.UtcNow;
        updateLayout.LastUpdated = now;
        updateLayout.LastUpdatedBy = currentUserId;
        Layout result = await layoutBroker.UpdateLayoutAsync(updateLayout);
        result.App = layout.App;
        return result;
    }

    public async ValueTask DeleteAsync(int id)
    {
        ValidateId(id, "id");
        Layout layout;
        try
        {
            layout = Get(id);
        }
        catch (SecurityException)
        {
            layout = Get(id, ignoreFilters: true);
        }

        if (layout == null)
        {
            return;
        }

        authorizationBroker.Authorize(layout.AppId, "Layout_delete");
        await layoutBroker.DeleteLayoutAsync(CreateStorageLayout(layout));
    }

    private static Layout CreateStorageLayout(Layout layout)
    {
        if (layout == null)
        {
            return null;
        }

        return new Layout
        {
            Id = layout.Id,
            Name = layout.Name,
            Description = layout.Description,
            LastUpdated = layout.LastUpdated,
            LastUpdatedBy = layout.LastUpdatedBy,
            CreatedOn = layout.CreatedOn,
            CreatedBy = layout.CreatedBy,
            AppId = layout.AppId,
            HeaderHtml = layout.HeaderHtml,
            Html = layout.Html,
            Script = layout.Script
        };
    }
}
