// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.IO;
using System.Threading.Tasks;
namespace cCoder.ContentManagement.Services.Processings;

internal interface ITemplateContentProcessingService
{
    ValueTask<string> ReadContentAsync(Stream source);
}