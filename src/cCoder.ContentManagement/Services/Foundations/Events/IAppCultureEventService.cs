// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.CMS;

namespace cCoder.ContentManagement.Services.Foundations.Events;

public interface IAppCultureEventService
{
    ValueTask RaiseAppCultureAddEventAsync(AppCulture entity);

    ValueTask RaiseAppCultureDeleteEventAsync(AppCulture entity);
}