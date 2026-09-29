// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Linq;
using cCoder.Data.Models.Security;

namespace cCoder.ContentManagement.Brokers.Storages;

internal interface IAppUserBroker
{
    IQueryable<User> GetAllAppUser(int appId);
}