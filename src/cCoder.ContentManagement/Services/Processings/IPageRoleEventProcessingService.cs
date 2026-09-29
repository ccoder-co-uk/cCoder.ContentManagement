// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Threading.Tasks;
using cCoder.Data.Models.Security;

namespace cCoder.ContentManagement.Services.Processings;

internal interface IPageRoleEventProcessingService
{
    ValueTask RaisePageRoleAddEventAsync(PageRole entity, string userId);

    ValueTask RaisePageRoleDeleteEventAsync(PageRole entity, string userId);
}