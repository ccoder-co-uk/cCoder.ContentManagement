// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Packaging;

namespace cCoder.ContentManagement.Services.Orchestrations;

public interface IMigrationSupportOrchestrationService
{
    T[] DeserializeItems<T>(string json);

    Package[] ExportPackages(int appId, string[] packageNames);
}