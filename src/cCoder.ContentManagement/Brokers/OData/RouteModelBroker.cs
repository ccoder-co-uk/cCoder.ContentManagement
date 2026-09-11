// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using Microsoft.OData.Edm;
using Microsoft.OData.ModelBuilder;

namespace cCoder.ContentManagement.Brokers.OData;

internal sealed class RouteModelBroker : IRouteModelBroker
{
    public IEdmModel BuildRouteModel(
        Action<ODataConventionModelBuilder> configureModel)
    {
        ODataConventionModelBuilder builder = new();
        configureModel(obj: builder);
        return builder.GetEdmModel();
    }
}