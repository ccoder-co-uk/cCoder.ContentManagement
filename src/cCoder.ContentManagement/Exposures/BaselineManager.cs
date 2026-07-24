// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Exposures.Setup;
using cCoder.Data.Models.Packaging;

namespace cCoder.ContentManagement.Exposures;

internal sealed class BaselineManager : IBaselineManager
{
    public Package[] GetPackages() =>
        UIBaseline.Packages;
}