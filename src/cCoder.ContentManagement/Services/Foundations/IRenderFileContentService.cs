// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

namespace cCoder.ContentManagement.Services.Foundations;

public interface IRenderFileContentService
{
    string GetLatestTextContent(int appId, string path);
}