// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Brokers;
using cCoder.Data.Models.Security;

namespace cCoder.ContentManagement.Services.Foundations.Authorization;

internal partial class AuthorizationService(
    IAuthorizationBroker authorizationBroker) : IAuthorizationService
{
    public User GetCurrentUser() =>
        TryCatch<User>(operation: () =>
            authorizationBroker.GetCurrentUser());
}