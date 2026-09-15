// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

namespace cCoder.ContentManagement.Brokers.Storages;

public interface IRenderFileContentBroker
{
    string GetLatestTextContent(int appId, string path);
}