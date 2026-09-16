// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Text;

namespace cCoder.ContentManagement.Brokers;

internal sealed class TemplateStreamBroker : ITemplateStreamBroker
{
    public async ValueTask<string> ReadAsync(Stream source)
    {
        using MemoryStream destination = new();
        await source.CopyToAsync(destination: destination);

        return Encoding.UTF8.GetString(bytes: destination.ToArray());
    }
}