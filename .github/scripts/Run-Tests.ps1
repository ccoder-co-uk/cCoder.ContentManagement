$ErrorActionPreference = "Stop"

$testProjects = @(
    "src/cCoder.ContentManagement.Tests/cCoder.ContentManagement.Tests.csproj",
    "src/ContentManagement.IntegrationTests/ContentManagement.IntegrationTests.csproj",
    "src/ContentManagement.AcceptanceTests/ContentManagement.AcceptanceTests.csproj"
)

New-Item -ItemType Directory -Path "artifacts/test-results" -Force | Out-Null

$processes = foreach ($project in $testProjects) {
    $resultName = [IO.Path]::GetFileNameWithoutExtension($project)
    Start-Process dotnet -NoNewWindow -PassThru -ArgumentList @(
        "test",
        $project,
        "-c", "Release",
        "--no-build",
        "--no-restore",
        "--logger", "trx;LogFileName=$resultName.trx",
        "--results-directory", "artifacts/test-results"
    )
}

$processes | Wait-Process
$failedProcesses = @($processes | Where-Object ExitCode -ne 0)

if ($failedProcesses.Count -ne 0) {
    throw "$($failedProcesses.Count) test project(s) failed."
}
