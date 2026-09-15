// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

namespace cCoder.ContentManagement.Dependencies;

internal sealed class TemplateStreamDependency : MemoryStream
{
    internal string ReadContent() =>
        System.Text.Encoding.UTF8.GetString(bytes: ToArray());
}