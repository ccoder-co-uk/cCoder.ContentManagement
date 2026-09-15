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
}