// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using Microsoft.OData.Edm;
using Microsoft.OData.ModelBuilder;

namespace cCoder.ContentManagement.Brokers.OData;

internal interface IRouteModelBroker
{
    IEdmModel BuildRouteModel(
        Action<ODataConventionModelBuilder> configureModel);
}