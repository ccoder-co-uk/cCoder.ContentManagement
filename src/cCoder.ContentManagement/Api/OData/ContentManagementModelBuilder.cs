using System.Linq.Expressions;
using cCoder.ContentManagement.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OData.Edm;
using Microsoft.OData.ModelBuilder;
using cCoder.Data.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Security;

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
