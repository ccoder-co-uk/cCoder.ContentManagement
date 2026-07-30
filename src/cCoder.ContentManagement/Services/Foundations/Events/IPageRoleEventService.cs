// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Security;

namespace cCoder.ContentManagement.Services.Foundations.Events;

internal interface IPageRoleEventService
{
    ValueTask RaisePageRoleAddEventAsync(PageRole entity);

    ValueTask RaisePageRoleDeleteEventAsync(PageRole entity);
}