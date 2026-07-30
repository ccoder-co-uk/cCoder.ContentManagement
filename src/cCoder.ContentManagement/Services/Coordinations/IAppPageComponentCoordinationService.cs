// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.CMS;

namespace cCoder.ContentManagement.Services.Coordinations;

internal interface IAppPageComponentCoordinationService
{
    ValueTask HandleAppAddAsync(App app);

    ValueTask HandleAppDeleteAsync(App app);

    ValueTask HandleAppUpdateAsync(App app);
}