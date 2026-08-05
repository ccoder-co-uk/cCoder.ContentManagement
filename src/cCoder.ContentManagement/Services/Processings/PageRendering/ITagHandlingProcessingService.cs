// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Models.PageRendering;

namespace cCoder.ContentManagement.Services.Processings.PageRendering;

internal interface ITagHandlingProcessingService
{
    TagHandlingOperation HandleTagHandlingOperation(
        TagHandlingOperation operation);
}