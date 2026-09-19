// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Models.Rendering;
using cCoder.ContentManagement.Brokers;

namespace cCoder.ContentManagement.Services.Foundations.Rendering;

internal sealed partial class PageRenderService(
    IJsonBroker jsonBroker,
    IFingerprintBroker fingerprintBroker)
        : IPageRenderService
{
    public PageRenderFoundationOperation ComputeFingerprintPageRenderFoundationOperation(
        PageRenderFoundationOperation pageRenderFoundationOperation) =>
        TryCatch(operation: () =>
        {
            ValidatePageRenderFoundationOperation(inputs: [pageRenderFoundationOperation]);

            pageRenderFoundationOperation.Json = fingerprintBroker.Compute(
                value: jsonBroker.Serialize(
                    value: pageRenderFoundationOperation.Value));

            return pageRenderFoundationOperation;
        });

    public PageRenderFoundationOperation SerializePageRenderFoundationOperation(
        PageRenderFoundationOperation pageRenderFoundationOperation) =>
        TryCatch(operation: () =>
        {
            ValidatePageRenderFoundationOperation(inputs: [pageRenderFoundationOperation]);

            pageRenderFoundationOperation.Json = jsonBroker
                .Serialize(value: pageRenderFoundationOperation.Value);

            return pageRenderFoundationOperation;
        });
}