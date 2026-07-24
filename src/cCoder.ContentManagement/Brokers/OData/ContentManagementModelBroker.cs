// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Linq.Expressions;
using cCoder.ContentManagement.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OData.Edm;
using Microsoft.OData.ModelBuilder;
using cCoder.Data.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Security;

namespace cCoder.ContentManagement.Brokers.OData;

internal class ContentManagementModelBroker
    : ODataModelBroker, IContentManagementModelBroker
{
    public ContentManagementModelBroker(ODataConventionModelBuilder builder = null)
        : base(builder)
    {
    }

    public override ODataModel Build() =>
        new ODataModel
        {
            Context = "Core",
            Description = "Content Management endpoints for the platform.",
            EDMModel = BuildEdmModel()
        };

    public void Configure() =>
        ConfigureModel();

    private IEdmModel BuildEdmModel()
    {
        ConfigureModel();
        return builder.GetEdmModel();
    }

    private void ConfigureModel()
    {
        AddCommonComplextypes();
        builder.ComplexType<RenderResult>();

        builder.EntityType<App>()
            .Ignore(propertyExpression: i => i.Config);

        builder.EntityType<Submission>()
            .Ignore(propertyExpression: i => i.Data);

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
        AddJoinSet(key: (Expression<Func<AppCulture, object>>)((AppCulture i) => new { i.AppId, i.CultureId }));
        AddJoinSet(key: (Expression<Func<PageRole, object>>)((PageRole i) => new { i.PageId, i.RoleId }));
        builder.Namespace = "";

        builder.EntityType<App>()
            .Function(name: "Users")
            .ReturnsCollection<User>();

        builder.EntityType<App>()
            .Action(name: "UpdatePageOrder")
            .Parameter<App>(name: "app");

        builder.EntityType<App>()
            .Function(name: "IsAdmin")
            .Returns<bool>();

        builder.EntityType<Page>()
            .Action(name: "AddContent")
            .Parameter<Content>(name: "content");

        builder.EntityType<Page>()
            .Function(name: "RootFor")
            .ReturnsFromEntitySet<Page>(entitySetName: "Page");

        builder.EntityType<Page>()
            .Function(name: "Menu")
            .Returns<Result<string>>();

        builder.EntityType<Page>()
            .Collection.Function(name: "Render")
            .Returns<RenderResult>();

        builder.EntityType<Resource>()
            .Collection.Function(name: "GetAll")
            .ReturnsCollectionFromEntitySet<Resource>(entitySetName: "Resource");

        builder.EntityType<Component>()
            .Collection.Function(name: "Render")
            .Returns<string>();

        builder.EntityType<Template>()
            .Collection.Action(name: "Render")
            .Returns<string>();

        builder.EntityType<Template>()
            .Collection.Action(name: "HtmlToPdf")
            .Returns<FileContentResult>();

        builder.EntityType<CommonObject>()
            .Collection.Function(name: "Latest")
            .ReturnsFromEntitySet<CommonObject>(entitySetName: "CommonObject");

        builder.EntityType<CommonObject>()
            .Collection.Action(name: "Import")
            .ReturnsCollectionFromEntitySet<Result<CommonObject>>(entitySetName: "ImportCommonObjectResults");
    }
}
