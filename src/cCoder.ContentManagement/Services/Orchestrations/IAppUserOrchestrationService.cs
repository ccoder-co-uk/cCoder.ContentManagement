// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Linq;
using cCoder.Data.Models.Security;

namespace cCoder.ContentManagement.Services.Orchestrations;

public interface IAppUserOrchestrationService
{
    IQueryable<User> GetAllUsers(int appId);
}