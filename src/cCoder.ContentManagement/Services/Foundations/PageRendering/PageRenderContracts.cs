// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------


using cCoder.ContentManagement.Models.PageRendering;

namespace cCoder.ContentManagement.Rendering.Services.Foundations;

internal interface IMetadataCacheService
{
    Func<string, string> Get(string culture);
}

internal interface ICommonObjectCacheService
{
    PageCacheSlice GetPageCacheSlice();
}

internal interface IMarkupRenderService
{
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