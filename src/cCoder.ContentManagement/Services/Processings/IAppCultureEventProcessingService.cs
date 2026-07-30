// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.CMS;

namespace cCoder.ContentManagement.Services.Processings;

internal interface IAppCultureEventProcessingService
{
    ValueTask RaiseAppCultureAddEventAsync(AppCulture entity);

    ValueTask RaiseAppCultureDeleteEventAsync(AppCulture entity);
}