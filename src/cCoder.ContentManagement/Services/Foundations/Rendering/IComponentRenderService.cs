// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

namespace cCoder.ContentManagement.Services.Foundations.Rendering;

internal interface IComponentRenderService
{
    string GetLatestTextContent(int appId, string path);
}