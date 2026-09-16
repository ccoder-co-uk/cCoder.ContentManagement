// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------


using cCoder.ContentManagement.Models.PageRendering;
using cCoder.ContentManagement.Models.Rendering;
using cCoder.ContentManagement.Models.Caching;
using cCoder.Data.Models;

namespace cCoder.ContentManagement.Rendering.Services.Foundations;

internal interface IMetadataCacheService
{
    MetadataCacheSnapshot GetMetadataCacheSnapshot();

    void SetMetadataCacheSnapshot(MetadataCacheSnapshot metadataCacheSnapshot);
}

internal interface IMetadataCacheSourceService
{
    MetadataCacheSnapshot BuildMetadataCacheSnapshot();

    string GetMetadataSignature();
}

internal interface IMetadataRenderCacheService
{
    Func<string, string> Get(string culture);
}

internal interface ICommonObjectCacheService
{
    CommonObjectCacheSnapshot GetCommonObjectCacheSnapshot();

    void SetCommonObjectCacheSnapshot(
        CommonObjectCacheSnapshot commonObjectCacheSnapshot,
        TimeSpan expiry);
}

internal interface IMarkupRenderService
{
    TagHandlingOperation PrepareRenderSessionTagHandlingOperation(
        TagHandlingOperation tagHandlingOperation);

    TagHandlingOperation HtmlEncodeTagHandlingOperation(
        TagHandlingOperation tagHandlingOperation);

    TagHandlingOperation GetPropertyValuesTagHandlingOperation(
        TagHandlingOperation tagHandlingOperation);

    string MarkContentSecurityPolicyNonce(string markup);

    TagHandlingOperation RenderCultureLinkTagHandlingOperation(TagHandlingOperation tagHandlingOperation);

    TagHandlingOperation RenderMetadataTagHandlingOperation(TagHandlingOperation tagHandlingOperation);

    TagHandlingOperation RenderNavigationTagHandlingOperation(TagHandlingOperation tagHandlingOperation);

    TagHandlingOperation RenderContentTagHandlingOperation(TagHandlingOperation tagHandlingOperation);

    TagHandlingOperation RenderComponentTagHandlingOperation(TagHandlingOperation tagHandlingOperation);

    TagHandlingOperation RenderScriptTagHandlingOperation(TagHandlingOperation tagHandlingOperation);

    TagHandlingOperation RenderStyleTagHandlingOperation(TagHandlingOperation tagHandlingOperation);

    TagHandlingOperation RenderDmsTagHandlingOperation(TagHandlingOperation tagHandlingOperation);

    TagHandlingOperation RenderResourceTagHandlingOperation(TagHandlingOperation tagHandlingOperation);

    TagHandlingOperation RenderExecuteTagHandlingOperation(TagHandlingOperation tagHandlingOperation);

    TagHandlingOperation SerializeTagHandlingOperation(TagHandlingOperation tagHandlingOperation);
    TagHandlingOperation ParseJsonTagHandlingOperation(TagHandlingOperation tagHandlingOperation);
    TagHandlingOperation NormalizeJsonTagHandlingOperation(TagHandlingOperation tagHandlingOperation);
    TagHandlingOperation IsJsonObjectTagHandlingOperation(TagHandlingOperation tagHandlingOperation);
    TagHandlingOperation IsJsonArrayTagHandlingOperation(TagHandlingOperation tagHandlingOperation);
    TagHandlingOperation IsJsonValueTagHandlingOperation(TagHandlingOperation tagHandlingOperation);
    TagHandlingOperation GetJsonPropertiesTagHandlingOperation(TagHandlingOperation tagHandlingOperation);
}

internal interface ICommonObjectLatestCacheService
{
    CommonObjectCacheSnapshot LoadCommonObjectCacheSnapshot();

    CommonObjectCacheSnapshot GetCommonObjectCacheSnapshot();
}

internal interface ICommonObjectRenderCacheService
{
    PageCacheSlice GetPageCacheSlice();
}