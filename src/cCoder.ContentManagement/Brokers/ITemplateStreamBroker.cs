// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

namespace cCoder.ContentManagement.Brokers;

internal interface ITemplateStreamBroker
{
    ValueTask<string> ReadAsync(Stream source);
}