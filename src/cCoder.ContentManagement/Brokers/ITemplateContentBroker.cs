// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

namespace cCoder.ContentManagement.Brokers;

public interface ITemplateContentBroker
{
    ValueTask<string> ReadAsync(Stream source);

    byte[] ConvertHtmlToPdf(string html);
}