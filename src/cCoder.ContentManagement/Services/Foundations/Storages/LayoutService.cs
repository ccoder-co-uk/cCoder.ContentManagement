// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Security;
using cCoder.ContentManagement.Brokers;
using cCoder.ContentManagement.Brokers.Storages;
using cCoder.Data.Models.CMS;

namespace cCoder.ContentManagement.Services.Foundations.Storages;

internal partial class LayoutService(ILayoutBroker layoutBroker, IAuthorizationBroker authorizationBroker) : ILayoutService
{
    public Layout GetLayout(int layoutId, bool ignoreFilters = false)
    {
        ValidateId(layoutId: layoutId, parameterName: "id");

        if (ignoreFilters)
        {
            return GetAllLayout(ignoreFilters: true)
                .FirstOrDefault(predicate: (Layout i) => i.Id == layoutId);
        }

        Layout layout = GetAllLayout()
            .FirstOrDefault(predicate: (Layout i) => i.Id == layoutId);

        if (layout != null)
        {
            return layout;
        }

        Layout layout2 = GetAllLayout(ignoreFilters: true)
            .FirstOrDefault(predicate: (Layout i) => i.Id == layoutId);

        if (layout2 != null)
        {
            throw new SecurityException(message: "Access Denied!");
        }

        return null;
    }

    public IQueryable<Layout> GetAllLayout(bool ignoreFilters = false) =>
        layoutBroker.GetAllLayouts(ignoreFilters: ignoreFilters);

    public async ValueTask<Layout> AddLayoutAsync(Layout layout)
    {
        ValidateLayout(layout: layout, parameterName: "layout");
        authorizationBroker.Authorize(appId: layout.AppId, privilege: "Layout_create");
        Layout newLayout = CreateStorageLayout(newLayout: layout);

        string currentUserId = authorizationBroker.GetCurrentUser()
            .Id;

        DateTimeOffset now = (newLayout.CreatedOn = DateTimeOffset.UtcNow);
        newLayout.CreatedBy = currentUserId;
        newLayout.LastUpdated = now;
        newLayout.LastUpdatedBy = currentUserId;
        Layout result = await layoutBroker.AddLayoutAsync(newLayout: newLayout);
        layout.Id = result.Id;
        layout.Name = result.Name;
        layout.Description = result.Description;
        layout.LastUpdated = result.LastUpdated;
        layout.LastUpdatedBy = result.LastUpdatedBy;
        layout.CreatedOn = result.CreatedOn;
        layout.CreatedBy = result.CreatedBy;
        layout.AppId = result.AppId;
        layout.HeaderHtml = result.HeaderHtml;
        layout.Html = result.Html;
        layout.Script = result.Script;
        return layout;
    }

    public async ValueTask<Layout> UpdateLayoutAsync(Layout updatedLayout)
    {
        ValidateLayout(layout: updatedLayout, parameterName: "layout");
        authorizationBroker.Authorize(appId: updatedLayout.AppId, privilege: "Layout_update");
        Layout updateLayout = CreateStorageLayout(newLayout: updatedLayout);

        string currentUserId = authorizationBroker.GetCurrentUser()
            .Id;

        DateTimeOffset now = DateTimeOffset.UtcNow;
        updateLayout.LastUpdated = now;
        updateLayout.LastUpdatedBy = currentUserId;
        Layout result = await layoutBroker.UpdateLayoutAsync(updatedLayout: updateLayout);
        updatedLayout.Id = result.Id;
        updatedLayout.Name = result.Name;
        updatedLayout.Description = result.Description;
        updatedLayout.LastUpdated = result.LastUpdated;
        updatedLayout.LastUpdatedBy = result.LastUpdatedBy;
        updatedLayout.CreatedOn = result.CreatedOn;
        updatedLayout.CreatedBy = result.CreatedBy;
        updatedLayout.AppId = result.AppId;
        updatedLayout.HeaderHtml = result.HeaderHtml;
        updatedLayout.Html = result.Html;
        updatedLayout.Script = result.Script;
        return updatedLayout;
    }

    public async ValueTask DeleteAsync(int layoutId)
    {
        ValidateId(layoutId: layoutId, parameterName: "id");
        Layout layout;

        try
        {
            layout = GetLayout(layoutId: layoutId);
        }
        catch (SecurityException)
        {
            layout = GetLayout(layoutId: layoutId, ignoreFilters: true);
        }

        if (layout == null)
        {
            return;
        }

        authorizationBroker.Authorize(appId: layout.AppId, privilege: "Layout_delete");
        await layoutBroker.DeleteLayoutAsync(deletedLayout: CreateStorageLayout(newLayout: layout));
    }

    private static Layout CreateStorageLayout(Layout newLayout)
    {
        if (newLayout == null)
        {
            return null;
        }

        return new Layout
        {
            Id = newLayout.Id,
            Name = newLayout.Name,
            Description = newLayout.Description,
            LastUpdated = newLayout.LastUpdated,
            LastUpdatedBy = newLayout.LastUpdatedBy,
            CreatedOn = newLayout.CreatedOn,
            CreatedBy = newLayout.CreatedBy,
            AppId = newLayout.AppId,
            HeaderHtml = newLayout.HeaderHtml,
            Html = newLayout.Html,
            Script = newLayout.Script
        };
    }
}