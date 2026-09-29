// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.IO;
using System.Threading.Tasks;
namespace cCoder.ContentManagement.Services.Orchestrations;

public interface ITemplateContentOrchestrationService
{
    ValueTask<string> ReadContentAsync(Stream source);

    byte[] ConvertHtmlToPdf(string html);
}