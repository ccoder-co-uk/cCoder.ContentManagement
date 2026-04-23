# cCoder.ContentManagement

`cCoder.ContentManagement` contains the Content Management domain for the cCoder platform.

## Contents

- `src/cCoder.ContentManagement`
  The main library package published to NuGet.
- `src/ContentManagement.Web`
  The standalone web host for the Content Management domain.
- `src/cCoder.ContentManagement.Tests`
  Unit tests for the domain.
- `src/ContentManagement.AcceptanceTests`
  Acceptance tests for the standalone host.

## Build

```powershell
dotnet build src/cCoder.ContentManagement.sln -v minimal
```

## Test

```powershell
dotnet test src/cCoder.ContentManagement.sln -v minimal --no-build
```

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
