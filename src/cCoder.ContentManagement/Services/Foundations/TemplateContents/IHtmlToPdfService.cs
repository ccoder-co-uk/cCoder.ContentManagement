// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

namespace cCoder.ContentManagement.Services.Foundations.TemplateContents;

internal interface IHtmlToPdfService
{
    byte[] ConvertHtmlToPdf(string html);
}