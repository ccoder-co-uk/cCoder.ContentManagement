// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Security;

namespace cCoder.ContentManagement.Services.Foundations.Events;

internal interface IPageRoleEventService
{
    ValueTask RaisePageRoleAddEventAsync(PageRole entity, string userId);

    ValueTask RaisePageRoleDeleteEventAsync(PageRole entity, string userId);
}