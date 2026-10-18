$ErrorActionPreference = 'Stop'
$repoRoot = Split-Path (Split-Path $PSScriptRoot -Parent) -Parent
$previewRoot = Join-Path $repoRoot 'Logs/AssetReview/CalfPreview'
New-Item -ItemType Directory -Force $previewRoot | Out-Null
$basePath = Join-Path $previewRoot 'base.gltf'
if (!(Test-Path -LiteralPath $basePath)) {
    Invoke-WebRequest 'https://drive.usercontent.google.com/download?id=1M9gyr2UikIDW_Ynp-OfgiK-JjI7ziPd3&export=download' -OutFile $basePath
}
$expected = '535DA2992EB125D517725159A2EDDEB520B72595BCB3021985FC43D2C84BFF76'
if ((Get-FileHash -LiteralPath $basePath).Hash -ne $expected) { throw 'Candidate source hash mismatch. Inspect the source before continuing.' }
$files = @{
    'build/three.module.js' = 'vendor/three.module.js'
    'examples/jsm/loaders/GLTFLoader.js' = 'vendor/addons/loaders/GLTFLoader.js'
    'examples/jsm/controls/OrbitControls.js' = 'vendor/addons/controls/OrbitControls.js'
    'examples/jsm/utils/BufferGeometryUtils.js' = 'vendor/addons/utils/BufferGeometryUtils.js'
    'LICENSE' = 'vendor/LICENSE.txt'
}
foreach ($entry in $files.GetEnumerator()) {
    $destination = Join-Path $previewRoot $entry.Value
    New-Item -ItemType Directory -Force (Split-Path $destination -Parent) | Out-Null
    if (!(Test-Path -LiteralPath $destination)) {
        Invoke-WebRequest ('https://cdn.jsdelivr.net/npm/three@0.160.1/' + $entry.Key) -OutFile $destination
    }
}
Copy-Item (Join-Path $PSScriptRoot 'index.html'),(Join-Path $PSScriptRoot 'preview.js'),(Join-Path $PSScriptRoot 'CalfModel.js') -Destination $previewRoot
Write-Output ('Preview prepared: ' + $previewRoot)
Write-Output 'Serve this directory on localhost only. No Unity asset has been imported.'
