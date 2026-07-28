// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using Microsoft.OData.Edm;
using Microsoft.OData.ModelBuilder;

namespace cCoder.ContentManagement.Extensions.OData;

internal static class ODataModelBuilderExtensions
{
    internal static IEdmModel BuildRouteModel(
        this Action<ODataConventionModelBuilder> configureModel)
    {
        ODataConventionModelBuilder builder = new();
        configureModel(obj: builder);
        return builder.GetEdmModel();
    }
}