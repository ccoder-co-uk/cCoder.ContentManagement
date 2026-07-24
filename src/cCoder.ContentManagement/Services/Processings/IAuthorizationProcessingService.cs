// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Models;

namespace cCoder.ContentManagement.Services.Processings;

public interface IAuthorizationProcessingService
{
    RenderAuthorization ResolveRenderAuthorization(string culture);
}