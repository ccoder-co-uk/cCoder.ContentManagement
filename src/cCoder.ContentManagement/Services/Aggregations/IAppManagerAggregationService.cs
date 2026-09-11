// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Models;

namespace cCoder.ContentManagement.Services.Aggregations;

internal interface IAppManagerAggregationService
{
    AppManagerContext GetAppManagerContext(AppManagerContext appManagerContext);

    AppManagerContext GetByDomainAppManagerContext(AppManagerContext appManagerContext);

    AppManagerContext GetAllAppManagerContext(AppManagerContext appManagerContext);

    ValueTask<AppManagerContext> AddAppManagerContextAsync(AppManagerContext newAppManagerContext);

    ValueTask<AppManagerContext> UpdateAppManagerContextAsync(AppManagerContext updatedAppManagerContext);

    ValueTask DeleteAppManagerContextAsync(AppManagerContext deletedAppManagerContext);

    AppManagerContext GetAdminAppManagerContext(AppManagerContext appManagerContext);

    AppManagerContext GetUsersAppManagerContext(AppManagerContext appManagerContext);

    ValueTask UpdatePageOrderAppManagerContextAsync(AppManagerContext updatedAppManagerContext);
}