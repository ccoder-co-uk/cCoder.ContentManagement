// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

namespace cCoder.ContentManagement.Services.Processings;

internal interface ITemplateContentProcessingService
{
    ValueTask<string> ReadContentAsync(Stream source);
}