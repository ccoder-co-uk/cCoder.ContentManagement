# cCoder.ContentManagement

`cCoder.ContentManagement` contains the Content Management domain for the cCoder platform.

## Functionality

The repository provides the content-management layer used by cCoder applications:

- App content structure, including app cultures, pages, page info, content, roles, components, layouts, resources, scripts, and templates.
- Package import support for applying packaged content definitions to an app.
- HTTP event receivers for app and page lifecycle events:
  - `app_add`, `app_update`, `app_delete`
  - `page_add`, `page_update`, `page_delete`
  - `package_import`
- A standalone Web host that exposes the Content Management API, OData endpoints, `/Health`, Swagger, and a simple manual test UI at `/tools/index.html`.

This repository does not currently host a separate Hosted Services app. Content lifecycle work is driven through the Web API and the HTTP eventing endpoint.

## Contents

- `src/cCoder.ContentManagement`
  The main library package published to NuGet.
- `src/ContentManagement.Web`
  The standalone web host for the Content Management domain. It hosts the API, event receivers, health check, Swagger, and manual test UI.
- `src/cCoder.ContentManagement.Tests`
  Unit tests for the domain.
- `src/ContentManagement.AcceptanceTests`
  Acceptance tests for the standalone host and manual test shell.
- `src/ContentManagement.IntegrationTests`
  Integration tests that call the HTTP eventing endpoint and verify app/page child relationship call chains.

## Build

```powershell
dotnet build src/cCoder.ContentManagement.sln -v minimal
```

## Test

```powershell
dotnet test src/cCoder.ContentManagement.sln -v minimal --no-build
```

The full solution test run includes unit tests, Web acceptance tests, and integration tests for the HTTP eventing chains. Relationship-focused integration tests assert each child set independently so app and page event regressions are easy to diagnose.

## Local Configuration

The standalone web host reads local secrets from environment variables rather than committed config.

Before running `src/ContentManagement.Web`, set:

- `ConnectionStrings__Core`
- `ConnectionStrings__SSO`
- `Settings__DecryptionKey`

The committed `appsettings.json` keeps these values blank so user or machine environment variables can supply them during local development.

## Local Run

```powershell
dotnet run --project src/ContentManagement.Web/ContentManagement.Web.csproj
```

Open the launched browser or navigate to:

- `https://localhost:7192/tools/index.html` for manual API and eventing checks.
- `https://localhost:7192/Health` for the health check.
- `https://localhost:7192/swagger` for API discovery.

The root path redirects to the manual tools page.

## Package

The NuGet package produced by this repository is:

- `cCoder.ContentManagement`

## Publishing

GitHub Actions is configured to publish the main package using NuGet trusted publishing.

Before the first publish, configure a trusted publishing policy on nuget.org for:

- Repository owner: `ccoder-co-uk`
- Repository: `cCoder.ContentManagement`
- Workflow file: `publish.yml`

The workflow also expects a `NUGET_USER` repository secret containing the nuget.org profile name used during trusted publishing login.
