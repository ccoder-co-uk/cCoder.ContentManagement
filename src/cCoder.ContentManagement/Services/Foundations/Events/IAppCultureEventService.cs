// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.CMS;

namespace cCoder.ContentManagement.Services.Foundations.Events;

internal interface IAppCultureEventService
{
    ValueTask RaiseAppCultureAddEventAsync(AppCulture entity, string userId);

    ValueTask RaiseAppCultureDeleteEventAsync(AppCulture entity, string userId);
}