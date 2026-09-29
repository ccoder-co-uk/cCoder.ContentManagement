// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Models.OData;
namespace cCoder.ContentManagement.Brokers.OData;

public interface IODataModelBroker
{
    ODataModel Build();
}