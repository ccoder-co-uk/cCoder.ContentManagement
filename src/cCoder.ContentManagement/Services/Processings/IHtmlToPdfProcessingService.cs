// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

namespace cCoder.ContentManagement.Services.Processings;

internal interface IHtmlToPdfProcessingService
{
    byte[] ConvertHtmlToPdf(string html);
}