// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Security;
using cCoder.ContentManagement.Brokers;
using cCoder.ContentManagement.Brokers.Storages;
using cCoder.Data.Models.CMS;

using cCoder.ContentManagement.Exposures;

namespace cCoder.ContentManagement.Services.Foundations.Storages;

internal partial class LayoutService(ILayoutBroker layoutBroker, IAuthorizationManager authorizationManager) : ILayoutService
{
    public Layout GetLayout(int layoutId, bool ignoreFilters = false) =>
        TryCatch<Layout>(operation: () =>
    {
        ValidateLayoutOnGet(inputs: [layoutId, ignoreFilters]);
        ValidateId(layoutId: layoutId, parameterName: "id");

        if (ignoreFilters)
        {
            return ExecuteGetAllLayout(ignoreFilters: true)
                .FirstOrDefault(predicate: (Layout i) => i.Id == layoutId);
        }

        Layout layout = ExecuteGetAllLayout()
            .FirstOrDefault(predicate: (Layout i) => i.Id == layoutId);

        if (layout != null)
        {
            return layout;
        }

        Layout layout2 = ExecuteGetAllLayout(ignoreFilters: true)
            .FirstOrDefault(predicate: (Layout i) => i.Id == layoutId);

        if (layout2 != null)
        {
            throw new SecurityException(message: "Access Denied!");
        }

        return null;

    });

    public IQueryable<Layout> GetAllLayout(bool ignoreFilters = false) =>
        TryCatch<IQueryable<Layout>>(operation: () =>
    {
        ValidateAllLayoutOnGet(inputs: [ignoreFilters]);

        return ignoreFilters
            ? layoutBroker.GetAllLayoutsIgnoringFilters()
            : layoutBroker.GetAllLayouts();
    });

    public ValueTask<Layout> AddLayoutAsync(Layout newLayout) =>
        TryCatch<Layout>(operation: async () =>
    {
        ValidateLayoutOnAdd(inputs: [newLayout]);
        ValidateLayout(layout: newLayout, parameterName: "layout");
        authorizationManager.Authorize(appId: newLayout.AppId, privilege: "Layout_create");
        Layout storageLayout = CreateStorageLayout(newLayout: newLayout);

        string currentUserId = authorizationManager.GetCurrentUser()
            .Id;

        DateTimeOffset now = (storageLayout.CreatedOn = DateTimeOffset.UtcNow);
        storageLayout.CreatedBy = currentUserId;
        storageLayout.LastUpdated = now;
        storageLayout.LastUpdatedBy = currentUserId;
        Layout result = await layoutBroker.AddLayoutAsync(newLayout: storageLayout);
        newLayout.Id = result.Id;
        newLayout.Name = result.Name;
        newLayout.Description = result.Description;
        newLayout.LastUpdated = result.LastUpdated;
        newLayout.LastUpdatedBy = result.LastUpdatedBy;
        newLayout.CreatedOn = result.CreatedOn;
        newLayout.CreatedBy = result.CreatedBy;
        newLayout.AppId = result.AppId;
        newLayout.HeaderHtml = result.HeaderHtml;
        newLayout.Html = result.Html;
        newLayout.Script = result.Script;
        return newLayout;

    }, isValueTask: true);

    public ValueTask<Layout> UpdateLayoutAsync(Layout updatedLayout) =>
        TryCatch<Layout>(operation: async () =>
    {
        ValidateLayoutOnUpdate(inputs: [updatedLayout]);
        ValidateLayout(layout: updatedLayout, parameterName: "layout");
        authorizationManager.Authorize(appId: updatedLayout.AppId, privilege: "Layout_update");
        Layout updateLayout = CreateStorageLayout(newLayout: updatedLayout);

        string currentUserId = authorizationManager.GetCurrentUser()
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

    }, isValueTask: true);

    public ValueTask DeleteAsync(int layoutId) =>
        TryCatch(operation: async () =>
    {
        ValidateDeleteAsync(inputs: [layoutId]);
        ValidateId(layoutId: layoutId, parameterName: "id");
        Layout layout;

        try
        {
            layout = ExecuteGetLayout(layoutId: layoutId);
        }
        catch (SecurityException)
        {
            layout = ExecuteGetLayout(layoutId: layoutId, ignoreFilters: true);
        }

        if (layout == null)
        {
            return;
        }

        authorizationManager.Authorize(appId: layout.AppId, privilege: "Layout_delete");
        await layoutBroker.DeleteLayoutAsync(deletedLayout: CreateStorageLayout(newLayout: layout));

    }, isValueTask: true);

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

    private IQueryable<Layout> ExecuteGetAllLayout(bool ignoreFilters = false) =>
        (ignoreFilters
            ? layoutBroker.GetAllLayoutsIgnoringFilters()
            : layoutBroker.GetAllLayouts());

    private Layout ExecuteGetLayout(int layoutId, bool ignoreFilters = false)
    {
        ValidateId(layoutId: layoutId, parameterName: "id");

        if (ignoreFilters)
        {
            return ExecuteGetAllLayout(ignoreFilters: true)
                .FirstOrDefault(predicate: (Layout i) => i.Id == layoutId);
        }

        Layout layout = ExecuteGetAllLayout()
            .FirstOrDefault(predicate: (Layout i) => i.Id == layoutId);

        if (layout != null)
        {
            return layout;
        }

        Layout layout2 = ExecuteGetAllLayout(ignoreFilters: true)
            .FirstOrDefault(predicate: (Layout i) => i.Id == layoutId);

        if (layout2 != null)
        {
            throw new SecurityException(message: "Access Denied!");
        }

        return null;
    }
}