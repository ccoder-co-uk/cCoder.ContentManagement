// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Rendering.Brokers;
using cCoder.Data.Models;

namespace cCoder.ContentManagement.Rendering.Services.Foundations;

internal sealed partial class CommonObjectLatestCacheService(
    ICommonObjectReaderBroker broker) : ICommonObjectLatestCacheService
{
    public void RefreshCommonObjects() =>
        TryCatch(operation: () => broker.Refresh());

    public IEnumerable<CommonObject> GetLatestCommonObjects() =>
        TryCatch(operation: () => broker.GetLatestSet());
}