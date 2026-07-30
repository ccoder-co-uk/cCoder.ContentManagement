// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

namespace cCoder.ContentManagement.Services.Foundations;

internal interface IRenderFileContentService
{
    string GetLatestTextContent(int appId, string path);
}