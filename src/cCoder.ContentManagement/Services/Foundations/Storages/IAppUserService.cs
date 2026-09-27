// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Security;

namespace cCoder.ContentManagement.Services.Foundations.Storages;

internal interface IAppUserService
{
    IQueryable<User> GetAllAppUser(int appId);
}