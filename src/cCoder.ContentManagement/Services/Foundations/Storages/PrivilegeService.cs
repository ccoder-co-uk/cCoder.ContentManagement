// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Brokers.Storages;
using cCoder.Data.Models.Security;

namespace cCoder.ContentManagement.Services.Foundations.Storages;

internal sealed partial class PrivilegeService(
    IPrivilegeBroker privilegeBroker) : IPrivilegeService
{
    public IQueryable<Privilege> GetAllPrivileges() =>
        TryCatch(operation: () =>
    {
        return privilegeBroker.GetAllPrivileges();
    });
}