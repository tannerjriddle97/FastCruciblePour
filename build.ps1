param([string]$GamePath = 'E:\Games\Vintagestory')
$ErrorActionPreference = 'Stop'
$runtimePath = Join-Path $env:ProgramFiles 'dotnet\shared\Microsoft.NETCore.App\10.0.7'
$sdkPath = Join-Path $env:ProgramFiles 'dotnet\sdk\9.0.302\Roslyn\bincore\csc.dll'
$buildPath = Join-Path $PSScriptRoot 'build'
New-Item -ItemType Directory -Path $buildPath -Force | Out-Null
$references = @(Get-ChildItem -LiteralPath $runtimePath -Filter '*.dll' | ForEach-Object {
    try { [Reflection.AssemblyName]::GetAssemblyName($_.FullName) | Out-Null; '/reference:' + $_.FullName } catch [System.BadImageFormatException] { }
})
$references += '/reference:' + (Join-Path $GamePath 'VintagestoryAPI.dll')
$references += '/reference:' + (Join-Path $GamePath 'Lib\0Harmony.dll')
& dotnet $sdkPath /nologo /target:library /optimize+ /langversion:latest /nostdlib+ @references ('/out:' + (Join-Path $buildPath 'FastCruciblePour.dll')) (Join-Path $PSScriptRoot 'FastCruciblePour.cs')
if ($LASTEXITCODE -ne 0) { throw 'Compilation failed.' }
Copy-Item -LiteralPath (Join-Path $PSScriptRoot 'modinfo.json') -Destination $buildPath
$distPath = Join-Path $PSScriptRoot 'dist'
New-Item -ItemType Directory -Path $distPath -Force | Out-Null
$zipPath = Join-Path $distPath 'FastCruciblePour-1.0.0.zip'
Compress-Archive -LiteralPath (Join-Path $buildPath 'FastCruciblePour.dll'), (Join-Path $buildPath 'modinfo.json') -DestinationPath $zipPath -Force
Write-Output "Built $zipPath"
