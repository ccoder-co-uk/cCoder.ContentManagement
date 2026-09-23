// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.CMS;

namespace cCoder.ContentManagement.Services.Processings;

internal interface IPageRenderDataProcessingService
{
    ValueTask<Page> GetPageForRenderAsync(int pageId);
}