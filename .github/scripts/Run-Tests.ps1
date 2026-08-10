param(
    [switch] $CollectCoverage
)

$ErrorActionPreference = "Stop"

$testProjects = @(
    "src/cCoder.ContentManagement.Tests/cCoder.ContentManagement.Tests.csproj",
    "src/ContentManagement.IntegrationTests/ContentManagement.IntegrationTests.csproj",
    "src/ContentManagement.AcceptanceTests/ContentManagement.AcceptanceTests.csproj"
)

New-Item -ItemType Directory -Path "artifacts/test-results" -Force | Out-Null

$testProcesses = foreach ($project in $testProjects) {
    $resultName = [IO.Path]::GetFileNameWithoutExtension($project)
    $arguments = @(
        "test",
        $project,
        "-c", "Release",
        "--no-build",
        "--no-restore",
        "--logger", "trx;LogFileName=$resultName.trx",
        "--results-directory", "artifacts/test-results"
    )

    $isAcceptanceProject =
        $project -like "*.AcceptanceTests/*"

    if ($CollectCoverage -and -not $isAcceptanceProject) {
        $arguments += @(
            "--collect", '"XPlat Code Coverage"',
            "--settings", "coverage.runsettings"
        )
    }

    $timeoutSeconds = if ($isAcceptanceProject) { 900 } else { 300 }
    $process = Start-Process dotnet -NoNewWindow -PassThru -ArgumentList $arguments

    Write-Host "Started '$project' as PID $($process.Id) with a $timeoutSeconds second timeout."

    [PSCustomObject]@{
        Project = $project
        Process = $process
        TimeoutSeconds = $timeoutSeconds
        Stopwatch = [Diagnostics.Stopwatch]::StartNew()
    }
}

$timedOutProjects = @()

while (@($testProcesses | Where-Object { -not $_.Process.HasExited }).Count -gt 0) {
    foreach ($testProcess in $testProcesses) {
        if (
            -not $testProcess.Process.HasExited -and
            $testProcess.Stopwatch.Elapsed.TotalSeconds -ge $testProcess.TimeoutSeconds
        ) {
            Write-Warning "Test project '$($testProcess.Project)' timed out after $($testProcess.TimeoutSeconds) seconds (PID $($testProcess.Process.Id))."
            Stop-Process -Id $testProcess.Process.Id -Force -ErrorAction SilentlyContinue
            $timedOutProjects += $testProcess.Project
        }
    }

    Start-Sleep -Seconds 1
}

$failedProcesses = @()

foreach ($testProcess in $testProcesses) {
    $testProcess.Process.WaitForExit()
    $testProcess.Stopwatch.Stop()

    Write-Host "Finished '$($testProcess.Project)' as PID $($testProcess.Process.Id) with exit code $($testProcess.Process.ExitCode) after $([Math]::Round($testProcess.Stopwatch.Elapsed.TotalSeconds, 1)) seconds."

    if ($testProcess.Process.ExitCode -ne 0) {
        $failedProcesses += $testProcess
    }
}

if ($timedOutProjects.Count -ne 0 -or $failedProcesses.Count -ne 0) {
    $failedProjectNames = @($failedProcesses | ForEach-Object Project)
    $failureSummary = @($timedOutProjects + $failedProjectNames | Sort-Object -Unique) -join ", "

    throw "Test projects failed or timed out: $failureSummary"
}
