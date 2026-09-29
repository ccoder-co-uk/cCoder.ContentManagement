// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Threading.Tasks;
using cCoder.ContentManagement.Models;

namespace cCoder.ContentManagement.Brokers.Storages;

internal interface IPageRenderDataBroker
{
    ValueTask<PageRenderData> GetPageRenderDataAsync(int pageId);
}