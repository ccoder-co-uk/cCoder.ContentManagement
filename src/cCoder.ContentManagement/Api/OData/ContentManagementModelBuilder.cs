using System.Linq.Expressions;
using cCoder.ContentManagement.Models;
using App = cCoder.Data.Models.CMS.App;
using AppCulture = cCoder.Data.Models.CMS.AppCulture;
using CommonObject = cCoder.Data.Models.CommonObject;
using Component = cCoder.Data.Models.CMS.Component;
using Content = cCoder.Data.Models.CMS.Content;
using Culture = cCoder.Data.Models.CMS.Culture;
using Layout = cCoder.Data.Models.CMS.Layout;
using MetaItem = cCoder.Data.Models.CMS.MetaItem;
using Page = cCoder.Data.Models.CMS.Page;
using PageInfo = cCoder.Data.Models.CMS.PageInfo;
using PageRole = cCoder.Data.Models.Security.PageRole;
using Resource = cCoder.Data.Models.CMS.Resource;
using Script = cCoder.Data.Models.CMS.Script;
using Submission = cCoder.Data.Models.CMS.Submission;
using Template = cCoder.Data.Models.CMS.Template;
using User = cCoder.Data.Models.Security.User;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OData.Edm;
using Microsoft.OData.ModelBuilder;

namespace cCoder.ContentManagement.Api.OData;

internal class ContentManagementModelBuilder : ODataModelBuilder
{
    public ContentManagementModelBuilder(ODataConventionModelBuilder builder = null)
        : base(builder)
    {
    }

    public override ODataModel Build()
    {
        return new ODataModel
        {
            Context = "Core",
            Description = "Content Management endpoints for the platform.",
            EDMModel = BuildEdmModel()
        };
    }

    public void Configure()
    {
        ConfigureModel();
    }

    private IEdmModel BuildEdmModel()
    {
        ConfigureModel();
        return base.Builder.GetEdmModel();
    }

    private void ConfigureModel()
    {
        AddCommonComplextypes();
        base.Builder.ComplexType<RenderResult>();
        base.Builder.EntityType<App>().Ignore(i => i.Config);
        base.Builder.EntityType<Submission>().Ignore(i => i.Data);
        AddSet<App, int>();
        AddSet<Layout, int>();
        AddSet<Template, int>();
        AddSet<Page, int>();
        AddSet<PageInfo, int>();
        AddSet<Content, int>();
        AddSet<Component, int>();
        AddSet<CommonObject, int>();
        AddSet<Script, int>();
        AddSet<MetaItem, int>();
        AddSet<Resource, int>();
        AddSet<Submission, Guid>();
        AddSet<Culture, string>();
        AddJoinSet((Expression<Func<AppCulture, object>>)((AppCulture i) => new { i.AppId, i.CultureId }));
        AddJoinSet((Expression<Func<PageRole, object>>)((PageRole i) => new { i.PageId, i.RoleId }));
        base.Builder.Namespace = "";
        base.Builder.EntityType<App>().Function("Users").ReturnsCollection<User>();
        base.Builder.EntityType<App>().Action("UpdatePageOrder").Parameter<App>("app");
        base.Builder.EntityType<App>().Function("IsAdmin").Returns<bool>();
        base.Builder.EntityType<Page>().Action("AddContent").Parameter<Content>("content");
        base.Builder.EntityType<Page>().Function("RootFor").ReturnsFromEntitySet<Page>("Page");
        base.Builder.EntityType<Page>().Function("Menu").Returns<Result<string>>();
        base.Builder.EntityType<Page>().Collection.Function("Render").Returns<RenderResult>();
        base.Builder.EntityType<Resource>().Collection.Function("GetAll").ReturnsCollectionFromEntitySet<Resource>("Resource");
        base.Builder.EntityType<Component>().Collection.Function("Render").Returns<string>();
        base.Builder.EntityType<Template>().Collection.Action("Render").Returns<string>();
        base.Builder.EntityType<Template>().Collection.Action("HtmlToPdf").Returns<FileContentResult>();
        base.Builder.EntityType<CommonObject>().Collection.Function("Latest").ReturnsFromEntitySet<CommonObject>("CommonObject");
        base.Builder.EntityType<CommonObject>().Collection.Action("Import").ReturnsCollectionFromEntitySet<Result<CommonObject>>("ImportCommonObjectResults");
    }
}
