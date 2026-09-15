// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Models.Rendering;

namespace cCoder.ContentManagement.Brokers.Rendering;

internal interface IRenderingUtilityBroker
{
    string HtmlEncode(string value);

    RuntimePropertyValue[] GetPropertyValues(object value);

    string ComputeFingerprint(string value);
}