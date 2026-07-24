// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Models;

namespace cCoder.ContentManagement.Services.Processings;

public interface IAuthorizationProcessingService
{
    void Authorize(int? appId, string privilege);

    bool IsAdmin(int appId, string userName);

    RenderAuthorization ResolveRenderAuthorization(string culture);

    bool IsAdminOfApp(int appId);
}