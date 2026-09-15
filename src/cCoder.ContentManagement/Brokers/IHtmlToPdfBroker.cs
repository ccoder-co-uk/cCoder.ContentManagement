// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

namespace cCoder.ContentManagement.Brokers;

internal interface IHtmlToPdfBroker
{
    byte[] ConvertHtmlToPdf(string html);
}