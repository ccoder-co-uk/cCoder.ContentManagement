namespace ContentManagement.Web;

public sealed class NoApiRouteConstraint : IRouteConstraint
{
    public bool Match(
        HttpContext httpContext,
        IRouter route,
        string routeKey,
        RouteValueDictionary values,
        RouteDirection routeDirection) =>
        httpContext.Request.Path.HasValue
        && !httpContext.Request.Path.Value.ToLowerInvariant().Contains("/api/");
}
