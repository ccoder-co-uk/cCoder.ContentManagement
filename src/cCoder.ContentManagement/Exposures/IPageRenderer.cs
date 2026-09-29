// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Models;
using System.Threading.Tasks;
namespace cCoder.ContentManagement.Exposures;

public interface IPageRenderer
{
    ValueTask<PageRenderResponse> RenderAsync();
}