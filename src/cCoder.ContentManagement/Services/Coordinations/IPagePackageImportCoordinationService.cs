// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.CMS;

namespace cCoder.ContentManagement.Services.Coordinations;

internal interface IPagePackageImportCoordinationService
{
    ValueTask ImportPagesAsync(int appId, Page[] pages);
}