// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Security;

namespace cCoder.ContentManagement.Services.Orchestrations;

public interface IAppUserOrchestrationService
{
    IQueryable<User> GetAllAppUser(int appId);
}