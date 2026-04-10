using System.Dynamic;
using cCoder.ContentManagement.Exposures;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using RenderApp = cCoder.Data.Models.CMS.App;
using RenderResult = cCoder.ContentManagement.Models.RenderResult;

namespace ContentManagement.Web.Controllers;

public sealed class HomeController(IPageRenderer PageRenderer) : Controller
{
    private string Host => Request.Host.Host.Replace("www.", "").ToLowerInvariant();

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
                    ((IDictionary<string, object>)result).Add(key, GetSessionValue(key));
            }

            return result;
        }
    }

    [HttpGet]
    public IActionResult Index(string path = null, string theme = null, string culture = null, bool edit = false)
    {
        try
        {
            if (path?.ToLowerInvariant().EndsWith(".php") == true)
            {
                Response.HttpContext.Abort();
                return Ok();
            }

            if (path?.ToLowerInvariant() == "robots.txt")
                return Content("User-agent: * Allow: *", "text/plain");

            if (!HttpContext.Session.IsAvailable)
                throw new Exception("Cannot load session information");

            culture = Response.HttpContext.Request.Query.ContainsKey("culture")
                ? Response.HttpContext.Request.Query["culture"].ToString()
                : null;

            if (culture != null)
                SetSessionValue("culture", culture);
            else
                culture = GetSessionValue("culture");

            if (theme != null)
                SetSessionValue("theme", theme);
            else
                theme = GetSessionValue("theme");

            PageRenderResponse response = PageRenderer.Render(
                new PageRenderRequest
                {
                    Host = Host,
                    Path = path,
                    Theme = theme,
                    Culture = culture,
                    Edit = edit,
                    RequestUrl = Request.GetEncodedUrl(),
                });

            SetSessionValue("theme", response.Theme);
            SetSessionValue("culture", response.Culture);
            SetupViewBag(response);

            ViewResult viewResult = View(response.Page);
            viewResult.StatusCode = response.Page.StatusCode;
            return viewResult;
        }
        catch (Exception ex)
        {
            return PartialView("Error", ex);
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

        session.page = page.KeyInfo();

        ViewData["Session"] = session;
        ViewData["Edit"] = response.Edit;
    }

    private string GetSessionValue(string key) =>
        HttpContext.Session.Keys.Contains(key.ToLowerInvariant())
            ? HttpContext.Session.GetString(key)
            : string.Empty;

    private void SetSessionValue(string key, string value)
    {
        if (value != null)
            HttpContext.Session.SetString(key.ToLowerInvariant(), value);
        else if (HttpContext.Session.Keys.Contains(key.ToLowerInvariant()))
            HttpContext.Session.Remove(key.ToLowerInvariant());
    }
}

