// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

namespace cCoder.ContentManagement.Services.Foundations.TemplateContents;

internal interface ITemplateContentService
{
    ValueTask<string> ReadContentAsync(Stream source);

}