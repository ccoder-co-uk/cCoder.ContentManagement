// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.CodeAnalysis.Exposures;
using cCoder.ContentManagement.Dependencies.Rendering;
using cCoder.ContentManagement.Models.Rendering;

namespace cCoder.ContentManagement.Brokers.Rendering;

internal sealed class RenderingUtilityBroker(
    RenderingUtilityDependency dependency = null)
        : IRenderingUtilityBroker, IUtilityBroker
{
    private readonly RenderingUtilityDependency dependency =
        dependency ?? new RenderingUtilityDependency();

    public string HtmlEncode(string value) =>
        dependency.HtmlEncode(value: value);

    public string Serialize(object value) =>
        dependency.Serialize(value: value);

    public RuntimePropertyValue[] GetPropertyValues(object value) =>
        dependency.GetPropertyValues(value: value);

    public string ComputeFingerprint(object value) =>
        dependency.ComputeFingerprint(value: value);
}