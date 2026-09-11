// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using Microsoft.OData.Edm;
using Microsoft.OData.ModelBuilder;
using cCoder.ContentManagement.Brokers.OData;

namespace cCoder.ContentManagement.Extensions.OData;

internal static class ActionExtensions
{
    internal static IEdmModel BuildRouteModel(
        this Action<ODataConventionModelBuilder> configureModel) =>
        new RouteModelBroker().BuildRouteModel(
            configureModel: configureModel);
}