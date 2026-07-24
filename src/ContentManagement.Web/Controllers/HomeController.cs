// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Dynamic;
using cCoder.ContentManagement.Exposures;
using cCoder.ContentManagement.Models;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using RenderApp = cCoder.Data.Models.CMS.App;
using RenderResult = cCoder.ContentManagement.Models.RenderResult;

namespace ContentManagement.Web.Controllers;

public sealed class HomeController(IPageRenderer PageRenderer) : Controller
{
    private string Host =>
        Request.Host.Host.Replace(oldValue: "www.", newValue: "")
        .ToLowerInvariant();

    private dynamic DynamicSessionObject
    {
        get
        {
            dynamic result = new ExpandoObject();

            result.apiRoot = (Request.Host.Port is not 443 and not 80)
                ? $"{Request.Scheme}://{Host}:{Request.Host.Port}/Api/"
                : $"{Request.Scheme}://{Host}/Api/";

            foreach (string key in HttpContext.Session.Keys)
            {
                if (key != "ssoUser")
                {
                    ((IDictionary<string, object>)result).Add(key: key, value: GetSessionValue(key: key));
                }
            }

            return result;
        }
    }

    [HttpGet]
    public IActionResult Index(string path = null, string theme = null, string culture = null, bool edit = false)
    {
        try
        {
            if (path?.ToLowerInvariant()
                .EndsWith(value: ".php") == true)
            {
                Response.HttpContext.Abort();
                return Ok();
            }

            if (path?.ToLowerInvariant() == "robots.txt")
            {
                return Content(content: "User-agent: * Allow: *", contentType: "text/plain");
            }

            if (!HttpContext.Session.IsAvailable)
            {
                throw new Exception(message: "Cannot load session information");
            }

            culture = Response.HttpContext.Request.Query.ContainsKey(key: "culture")
                ? Response.HttpContext.Request.Query["culture"].ToString()
                : null;

            if (culture != null)
            {
                SetSessionValue(key: "culture", value: culture);
            }
            else
            {
                culture = GetSessionValue(key: "culture");
            }

            if (theme != null)
            {
                SetSessionValue(key: "theme", value: theme);
            }
            else
            {
                theme = GetSessionValue(key: "theme");
            }

            PageRenderResponse response = PageRenderer.Render(
request: new PageRenderRequest
{
    Host = Host,
    Path = path,
    Theme = theme,
    Culture = culture,
    Edit = edit,
    RequestUrl = Request.GetEncodedUrl(),
});

            SetSessionValue(key: "theme", value: response.Theme);
            SetSessionValue(key: "culture", value: response.Culture);
            SetupViewBag(response: response);

            ViewResult viewResult = View(model: response.Page);
            viewResult.StatusCode = response.Page.StatusCode;
            return viewResult;
        }
        catch (Exception ex)
        {
            return PartialView(viewName: "Error", model: ex);
        }
    }

    private void SetupViewBag(PageRenderResponse response)
    {
        dynamic session = DynamicSessionObject;

        RenderApp app = response.App;
        RenderResult page = response.Page;

        session.app = new
        {
            app.Id,
            app.TenantId,
            app.Domain,
            app.DefaultCultureId,
            app.DefaultTheme,
            app.Config
        };

        session.page = new
        {
            page.AppId,
            page.PageId,
            page.ParentId
        };

        ViewData["Session"] = session;
        ViewData["Edit"] = response.Edit;
    }

    private string GetSessionValue(string key) =>
        HttpContext.Session.Keys.Contains(value: key.ToLowerInvariant())
            ? HttpContext.Session.GetString(key: key)
            : string.Empty;

    private void SetSessionValue(string key, string value)
    {
        if (value != null)
        {
            HttpContext.Session.SetString(key: key.ToLowerInvariant(), value: value);
        }
        else
        {
            if (HttpContext.Session.Keys.Contains(value: key.ToLowerInvariant()))
            {
                HttpContext.Session.Remove(key: key.ToLowerInvariant());
            }
        }
    }
}