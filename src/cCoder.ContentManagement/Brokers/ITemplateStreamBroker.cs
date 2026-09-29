// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.IO;
using System.Threading.Tasks;
namespace cCoder.ContentManagement.Brokers;

internal interface ITemplateStreamBroker
{
    ValueTask<string> ReadAsync(Stream source);
}