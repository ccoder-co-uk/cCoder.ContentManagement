// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.IO;
using System.Threading.Tasks;
namespace cCoder.ContentManagement.Services.Foundations.TemplateContents;

internal interface ITemplateContentService
{
    ValueTask<string> ReadContentAsync(Stream source);

}