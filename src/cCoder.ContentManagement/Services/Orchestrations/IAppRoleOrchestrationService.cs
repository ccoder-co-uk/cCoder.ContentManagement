// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.CMS;

namespace cCoder.ContentManagement.Services.Orchestrations;

internal interface IAppRoleOrchestrationService
{
    ValueTask PersistNewAppRolesAsync(App app);
}