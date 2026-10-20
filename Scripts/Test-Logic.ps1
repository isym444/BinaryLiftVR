param([string]$UnityEditorRoot = 'D:\Unity\6000.0.85f1\Editor')
$ErrorActionPreference = 'Stop'
$projectRoot = Split-Path $PSScriptRoot -Parent
$monoPath = Join-Path $UnityEditorRoot 'Data\MonoBleedingEdge\bin\mono.exe'
$compilerPath = Join-Path $UnityEditorRoot 'Data\MonoBleedingEdge\lib\mono\4.5\csc.exe'
if (!(Test-Path $monoPath) -or !(Test-Path $compilerPath)) {
    throw 'Unity Mono compiler not found. Pass -UnityEditorRoot or use dotnet run --project LogicTests.'
}
$outputDirectory = Join-Path $projectRoot 'Logs'
New-Item -ItemType Directory -Force -Path $outputDirectory | Out-Null
$outputPath = Join-Path $outputDirectory 'LogicTests.exe'
$sources = @(Get-ChildItem (Join-Path $projectRoot 'Assets\Scripts\Logic') -Filter '*.cs' | ForEach-Object { $_.FullName })
$sources += Join-Path $projectRoot 'LogicTests\Program.cs'
& $monoPath $compilerPath /nologo /target:exe "/out:$outputPath" @sources
if ($LASTEXITCODE -ne 0) { throw 'Logic test compilation failed.' }
& $monoPath $outputPath
if ($LASTEXITCODE -ne 0) { throw 'Logic tests failed.' }
