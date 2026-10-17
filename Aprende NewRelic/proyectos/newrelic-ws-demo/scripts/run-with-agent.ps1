# Arranca la app con el profiler de New Relic adjunto.
# Lanza el .exe directamente (evita instrumentar también el proceso `dotnet`).
#
# Uso:
#   pwsh ./scripts/run-with-agent.ps1
#   pwsh ./scripts/run-with-agent.ps1 -Configuration Release

param(
    [ValidateSet("Debug", "Release")]
    [string] $Configuration = "Debug"
)

$ErrorActionPreference = "Stop"

$demoRoot = Split-Path $PSScriptRoot -Parent
$projectDir = Join-Path $demoRoot "src\NewRelicWsDemo"
$csproj = Join-Path $projectDir "NewRelicWsDemo.csproj"

if (-not (Test-Path $csproj)) {
    throw "No se encontró el proyecto en $csproj"
}

$outDir = Join-Path $projectDir "bin\$Configuration\net8.0"
$nrHome = Join-Path $outDir "newrelic"
$profiler = Join-Path $nrHome "NewRelic.Profiler.dll"
$exe = Join-Path $outDir "NewRelicWsDemo.exe"

Write-Host "Compilando ($Configuration)..."
dotnet build $csproj -c $Configuration --nologo -v q
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }

if (-not (Test-Path $profiler)) {
    throw "No se encontró el profiler en $profiler. ¿Está el paquete NewRelic.Agent?"
}
if (-not (Test-Path $exe)) {
    throw "No se encontró $exe"
}

$env:CORECLR_ENABLE_PROFILING = "1"
$env:CORECLR_PROFILER = "{36032161-FFC0-4B61-B559-F6C5D41BAE5A}"
$env:CORECLR_PROFILER_PATH = $profiler
$env:CORECLR_NEWRELIC_HOME = $nrHome

# Licencia / app / host: preferir entorno ya definido; si no, leer appsettings.json
$appsettings = Join-Path $projectDir "appsettings.json"
if (Test-Path $appsettings) {
    $json = Get-Content $appsettings -Raw | ConvertFrom-Json
    if (-not $env:NEW_RELIC_LICENSE_KEY) { $env:NEW_RELIC_LICENSE_KEY = $json.NEW_RELIC_LICENSE_KEY }
    if (-not $env:NEW_RELIC_APP_NAME)    { $env:NEW_RELIC_APP_NAME    = $json.NEW_RELIC_APP_NAME }
    if (-not $env:NEW_RELIC_HOST)        { $env:NEW_RELIC_HOST        = $json.NEW_RELIC_HOST }
}

$env:NEW_RELIC_DISTRIBUTED_TRACING_ENABLED = "true"
$env:NEW_RELIC_APPLICATION_LOGGING_ENABLED = "true"
$env:NEW_RELIC_APPLICATION_LOGGING_FORWARDING_ENABLED = "true"
$env:NEW_RELIC_LOG = "stdout"
if (-not $env:ASPNETCORE_ENVIRONMENT) { $env:ASPNETCORE_ENVIRONMENT = "Development" }
$env:ASPNETCORE_URLS = if ($env:ASPNETCORE_URLS) { $env:ASPNETCORE_URLS } else { "http://localhost:5000" }

Write-Host "Profiler: $profiler"
Write-Host "App:      $($env:NEW_RELIC_APP_NAME) @ $($env:NEW_RELIC_HOST)"
Write-Host "Arrancando $exe ..."
Write-Host "Logs NR:  $(Join-Path $nrHome 'logs\stdout')"

Set-Location $outDir
& $exe
