// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.CMS;

namespace cCoder.ContentManagement.Services.Orchestrations;

internal interface IAppBootstrapOrchestrationService
{
    App PrepareNewApp(App app, bool isFirstApp);

    void StampAppChildren(App app);
}