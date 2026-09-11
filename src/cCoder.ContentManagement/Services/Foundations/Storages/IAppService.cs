// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Models;

namespace cCoder.ContentManagement.Services.Foundations.Storages;

internal interface IAppService
{
    AppOperation GetVisibleAppsAppOperation(AppOperation appOperation);
    AppOperation GetUnfilteredAppsAppOperation(AppOperation appOperation);
    AppOperation GetVisibleAppAppOperation(AppOperation appOperation);
    AppOperation GetUnfilteredAppAppOperation(AppOperation appOperation);
    ValueTask<AppOperation> GetAppForRenderAppOperationAsync(AppOperation appOperation);
    AppOperation GetAppForDeleteAppOperation(AppOperation appOperation);
    ValueTask<AppOperation> AddAppOperationAsync(AppOperation newAppOperation);
    ValueTask<AppOperation> UpdateAppOperationAsync(AppOperation updatedAppOperation);
    ValueTask<AppOperation> DeleteAppOperationAsync(AppOperation deletedAppOperation);
    AppOperation GetRequestPathAppOperation(AppOperation appOperation);
    AppOperation GetRequestHostAppOperation(AppOperation appOperation);
    AppOperation GetCulturesAppOperation(AppOperation appOperation);
    AppOperation GetPrivilegesAppOperation(AppOperation appOperation);
    AppOperation GetCurrentUserAppOperation(AppOperation appOperation);
    AppOperation GetCurrentUserIdAppOperation(AppOperation appOperation);
    AppOperation IsAdminOfAppAppOperation(AppOperation appOperation);
    AppOperation AuthorizeAppOperation(AppOperation appOperation);
    ValueTask<AppOperation> AddRoleAppOperationAsync(AppOperation newAppOperation);
    ValueTask<AppOperation> UpdateRoleAppOperationAsync(AppOperation updatedAppOperation);
    AppOperation GetRolesAppOperation(AppOperation appOperation);
    ValueTask<AppOperation> AddUserRoleAppOperationAsync(AppOperation newAppOperation);
    AppOperation GetUserRolesAppOperation(AppOperation appOperation);
    ValueTask<AppOperation> DeleteUserRolesAppOperationAsync(AppOperation deletedAppOperation);
    AppOperation GetPagesAppOperation(AppOperation appOperation);
    ValueTask<AppOperation> UpdatePageAppOperationAsync(AppOperation updatedAppOperation);
}