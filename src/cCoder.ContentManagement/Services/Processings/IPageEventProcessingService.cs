// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Threading.Tasks;
using cCoder.Data.Models.CMS;

namespace cCoder.ContentManagement.Services.Processings;

internal interface IPageEventProcessingService
{
    ValueTask RaisePageAddEventAsync(Page entity, string userId);

    ValueTask RaisePageUpdateEventAsync(Page entity, string userId);

    ValueTask RaisePageDeleteEventAsync(Page entity, string userId);

}