// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Brokers;
using Microsoft.OData.ModelBuilder;

namespace cCoder.ContentManagement.Extensions.OData;

public static class ContentManagementApiModelExtensions
{
    public static void ConfigureContentManagementApiModel(
        this ODataConventionModelBuilder builder) =>
        new ContentManagementModelBroker(builder: builder).Configure();
}