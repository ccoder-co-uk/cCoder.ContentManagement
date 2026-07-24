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

        base.Builder.EntityType<App>()
            .Ignore(propertyExpression: i => i.Config);

        base.Builder.EntityType<Submission>()
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
        base.Builder.Namespace = "";

        base.Builder.EntityType<App>()
            .Function(name: "Users")
            .ReturnsCollection<User>();

        base.Builder.EntityType<App>()
            .Action(name: "UpdatePageOrder")
            .Parameter<App>(name: "app");

        base.Builder.EntityType<App>()
            .Function(name: "IsAdmin")
            .Returns<bool>();

        base.Builder.EntityType<Page>()
            .Action(name: "AddContent")
            .Parameter<Content>(name: "content");

        base.Builder.EntityType<Page>()
            .Function(name: "RootFor")
            .ReturnsFromEntitySet<Page>(entitySetName: "Page");

        base.Builder.EntityType<Page>()
            .Function(name: "Menu")
            .Returns<Result<string>>();

        base.Builder.EntityType<Page>()
            .Collection.Function(name: "Render")
            .Returns<RenderResult>();

        base.Builder.EntityType<Resource>()
            .Collection.Function(name: "GetAll")
            .ReturnsCollectionFromEntitySet<Resource>(entitySetName: "Resource");

        base.Builder.EntityType<Component>()
            .Collection.Function(name: "Render")
            .Returns<string>();

        base.Builder.EntityType<Template>()
            .Collection.Action(name: "Render")
            .Returns<string>();

        base.Builder.EntityType<Template>()
            .Collection.Action(name: "HtmlToPdf")
            .Returns<FileContentResult>();

        base.Builder.EntityType<CommonObject>()
            .Collection.Function(name: "Latest")
            .ReturnsFromEntitySet<CommonObject>(entitySetName: "CommonObject");

        base.Builder.EntityType<CommonObject>()
            .Collection.Action(name: "Import")
            .ReturnsCollectionFromEntitySet<Result<CommonObject>>(entitySetName: "ImportCommonObjectResults");
    }
}