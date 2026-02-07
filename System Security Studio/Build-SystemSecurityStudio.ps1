<#
.SYNOPSIS
    Local build script for System Security Studio MSIX package.

.DESCRIPTION
    Builds the System Security Studio application locally, creating a signed MSIX package.
    Detects installed tools and only installs missing dependencies.

.PARAMETER SkipClean
    Skip cleaning previous build artifacts.

.PARAMETER Install
    Install the built package after building.

.EXAMPLE
    .\Build-SystemSecurityStudio.ps1
    Build for x64.

.EXAMPLE
    .\Build-SystemSecurityStudio.ps1 -Install
    Build x64 and install the package.
#>

#Requires -Version 7.0
#Requires -RunAsAdministrator

[CmdletBinding()]
param(
    [switch]$SkipClean,
    [switch]$Install
)

$ErrorActionPreference = 'Stop'
$Stopwatch = [System.Diagnostics.Stopwatch]::StartNew()

# Configuration (matches the Store publisher for System Security Studio)
$PackagePublisher = 'CN=C62E63B6-6EF1-4F86-B80F-41A725BD0189'
$PackageHashAlgo = 'SHA256'

Write-Host "`n========================================" -ForegroundColor Cyan
Write-Host "  System Security Studio - Local Build" -ForegroundColor Cyan
Write-Host "========================================`n" -ForegroundColor Cyan

#region --- Check Prerequisites ---
Write-Host "Checking prerequisites..." -ForegroundColor Magenta

# Enable long paths
Set-ItemProperty -Path 'HKLM:\SYSTEM\CurrentControlSet\Control\FileSystem' -Name 'LongPathsEnabled' -Value 1 -Force -ErrorAction SilentlyContinue

# Check .NET SDK
$dotnetVersion = $null
try {
    $dotnetVersion = dotnet --version 2>$null
    Write-Host "  [OK] .NET SDK: $dotnetVersion" -ForegroundColor Green
} catch {
    Write-Host "  [MISSING] .NET SDK - Please install .NET 10 SDK" -ForegroundColor Red
    Write-Host "    winget install --id Microsoft.DotNet.SDK.10" -ForegroundColor Yellow
    exit 1
}

# Check Rust
$rustVersion = $null
try {
    $rustVersion = rustc --version 2>$null
    Write-Host "  [OK] Rust: $rustVersion" -ForegroundColor Green
} catch {
    Write-Host "  [MISSING] Rust toolchain - Please install Rustup" -ForegroundColor Red
    Write-Host "    winget install --id Rustlang.Rustup" -ForegroundColor Yellow
    exit 1
}

# Check Visual Studio / Build Tools
$vsWherePath = 'C:\Program Files (x86)\Microsoft Visual Studio\Installer\vswhere.exe'
if (-not (Test-Path $vsWherePath)) {
    Write-Host "  [MISSING] Visual Studio Installer (vswhere.exe) not found" -ForegroundColor Red
    Write-Host "    Install Visual Studio Build Tools or Visual Studio" -ForegroundColor Yellow
    exit 1
}

$installationPath = & $vsWherePath -prerelease -latest -property installationPath -products *
if (-not $installationPath) {
    Write-Host "  [MISSING] Visual Studio or Build Tools not found" -ForegroundColor Red
    exit 1
}
Write-Host "  [OK] Visual Studio: $installationPath" -ForegroundColor Green

# Check MSBuild
$MSBuildPath = [System.IO.Path]::Combine($installationPath, 'MSBuild', 'Current', 'Bin', 'MSBuild.exe')
if (-not (Test-Path $MSBuildPath)) {
    Write-Host "  [MISSING] MSBuild.exe not found" -ForegroundColor Red
    exit 1
}
Write-Host "  [OK] MSBuild: $MSBuildPath" -ForegroundColor Green

# Check Windows SDK (for MakeAppx and SignTool)
$WindowsKitsPath = 'C:\Program Files (x86)\Windows Kits\10\bin'
if (-not (Test-Path $WindowsKitsPath)) {
    Write-Host "  [MISSING] Windows SDK not found" -ForegroundColor Red
    exit 1
}

# Find latest SDK version
$sdkVersions = Get-ChildItem -Path $WindowsKitsPath -Directory | Where-Object { $_.Name -match '^\d+\.\d+\.\d+\.\d+$' } | Sort-Object { [version]$_.Name } -Descending
if ($sdkVersions.Count -eq 0) {
    Write-Host "  [MISSING] No Windows SDK versions found" -ForegroundColor Red
    exit 1
}
$latestSdkVersion = $sdkVersions[0].Name
$arch = if ($env:PROCESSOR_ARCHITECTURE -eq 'ARM64') { 'arm64' } else { 'x64' }
$MakeAppxPath = [System.IO.Path]::Combine($WindowsKitsPath, $latestSdkVersion, $arch, 'makeappx.exe')
$SignToolPath = [System.IO.Path]::Combine($WindowsKitsPath, $latestSdkVersion, $arch, 'signtool.exe')

if (-not (Test-Path $MakeAppxPath)) { Write-Host "  [MISSING] MakeAppx.exe not found" -ForegroundColor Red; exit 1 }
if (-not (Test-Path $SignToolPath)) { Write-Host "  [MISSING] SignTool.exe not found" -ForegroundColor Red; exit 1 }
Write-Host "  [OK] Windows SDK: $latestSdkVersion" -ForegroundColor Green

# Find mspdbcmf.exe for NativeAOT
$VCToolsPath = [System.IO.Path]::Combine($installationPath, 'VC', 'Tools', 'MSVC')
$vcVersions = Get-ChildItem -Path $VCToolsPath -Directory | Sort-Object { [version]$_.Name } -Descending -ErrorAction SilentlyContinue

$mspdbcmfPath = $null
if ($vcVersions.Count -gt 0) {
    $mspdbcmfPath = [System.IO.Path]::Combine($VCToolsPath, $vcVersions[0].Name, 'bin', 'Hostx64', 'x64', 'mspdbcmf.exe')
    if (Test-Path $mspdbcmfPath) {
        Write-Host "  [OK] mspdbcmf: Found" -ForegroundColor Green
    }
}
#endregion

#region --- Setup VS Developer Environment ---
Write-Host "`nSetting up VS Developer Environment..." -ForegroundColor Magenta
$vsDevCmdPath = "$installationPath\Common7\Tools\vsdevcmd.bat"
if (Test-Path $vsDevCmdPath) {
    & "${env:COMSPEC}" /s /c "`"$vsDevCmdPath`" -no_logo && set" | ForEach-Object {
        if ($_ -match '^([^=]+)=(.*)$') {
            Set-Content -Path "env:\$($Matches[1])" -Value $Matches[2] -Force
        }
    }
}

# Refresh PATH
$Env:Path = [System.Environment]::GetEnvironmentVariable('Path', 'Machine') + ';' + [System.Environment]::GetEnvironmentVariable('Path', 'User')
#endregion

#region --- Clean Previous Build ---
if (-not $SkipClean) {
    Write-Host "`nCleaning previous build artifacts..." -ForegroundColor Magenta
    @('MSIXOutputX64', 'MSIXBundleOutput', 'bin', 'obj') | ForEach-Object {
        Remove-Item -Path ".\$_" -Recurse -Force -ErrorAction Ignore
    }
}
#endregion

#region --- Build C++ Projects (from App Control Studio) ---
Write-Host "`nBuilding C++ projects..." -ForegroundColor Magenta

# ComManager (shared with App Control Studio)
Write-Host "  Building ComManager (x64)..." -ForegroundColor Yellow
& $MSBuildPath '..\App Control Studio\eXclude\ComManager\ComManager.slnx' /p:Configuration=Release /p:Platform=x64 /target:"clean;Rebuild" /v:minimal
if ($LASTEXITCODE -ne 0) { throw "Failed building ComManager for x64" }
#endregion

#region --- Build Rust Projects ---
Write-Host "`nBuilding Rust projects..." -ForegroundColor Magenta

# Setup Rust toolchain
rustup default nightly 2>$null
rustup target add x86_64-pc-windows-msvc 2>$null
rustup component add rust-src --toolchain nightly-x86_64-pc-windows-msvc 2>$null

Push-Location '..\App Control Studio\eXclude\Rust Interop Library'

Write-Host "  Building Rust Interop (x64)..." -ForegroundColor Yellow
cargo build_x64
if ($LASTEXITCODE -ne 0) { Pop-Location; throw "Failed building x64 Rust Interop project" }

Pop-Location
#endregion

#region --- Build C# Service Projects ---
Write-Host "`nBuilding C# service projects..." -ForegroundColor Magenta

# DISM Service
Write-Host "  Building DISMService (x64)..." -ForegroundColor Yellow
dotnet restore '..\App Control Studio\eXclude\DISMService\DISMService.csproj' -r win-x64
dotnet clean '..\App Control Studio\eXclude\DISMService\DISMService.csproj' --configuration Release
dotnet build '..\App Control Studio\eXclude\DISMService\DISMService.csproj' --configuration Release --verbosity minimal /p:Platform=x64 /p:RuntimeIdentifier=win-x64
if ($LASTEXITCODE -ne 0) { throw "Failed building DISMService x64" }
dotnet msbuild '..\App Control Studio\eXclude\DISMService\DISMService.csproj' /p:Configuration=Release /restore /p:Platform=x64 /p:RuntimeIdentifier=win-x64 /p:PublishProfile="..\App Control Studio\eXclude\DISMService\Properties\PublishProfiles\win-x64.pubxml" /t:Publish -v:minimal
if ($LASTEXITCODE -ne 0) { throw "Failed publishing DISMService x64" }

# QuantumRelay Service
Write-Host "  Building QuantumRelayHSS (x64)..." -ForegroundColor Yellow
dotnet restore '..\App Control Studio\eXclude\QuantumRelayHSS\QuantumRelayHSS.csproj' -r win-x64
dotnet clean '..\App Control Studio\eXclude\QuantumRelayHSS\QuantumRelayHSS.csproj' --configuration Release
dotnet build '..\App Control Studio\eXclude\QuantumRelayHSS\QuantumRelayHSS.csproj' --configuration Release --verbosity minimal /p:Platform=x64 /p:RuntimeIdentifier=win-x64
if ($LASTEXITCODE -ne 0) { throw "Failed building QuantumRelayHSS x64" }
dotnet msbuild '..\App Control Studio\eXclude\QuantumRelayHSS\QuantumRelayHSS.csproj' /p:Configuration=Release /restore /p:Platform=x64 /p:RuntimeIdentifier=win-x64 /p:PublishProfile="..\App Control Studio\eXclude\QuantumRelayHSS\Properties\PublishProfiles\win-x64.pubxml" /t:Publish -v:minimal
if ($LASTEXITCODE -ne 0) { throw "Failed publishing QuantumRelayHSS x64" }
#endregion

#region --- Build .NET MSIX ---
Write-Host "`nBuilding MSIX packages..." -ForegroundColor Magenta

# Copy x64 native components
Copy-Item -Path '..\App Control Studio\eXclude\ComManager\x64\Release\ComManager.exe' -Destination '.\CppInterop\ComManager.exe' -Force
Copy-Item -Path '..\App Control Studio\eXclude\DISMService\OutputX64\DISMService.exe' -Destination '.\DISMService.exe' -Force
Copy-Item -Path '..\App Control Studio\eXclude\QuantumRelayHSS\OutputX64\QuantumRelayHSS.exe' -Destination '.\QuantumRelayHSS.exe' -Force

Write-Host "  Building x64 MSIX..." -ForegroundColor Yellow
dotnet clean 'System Security Studio.csproj' --configuration Release
dotnet build 'System Security Studio.csproj' --configuration Release --verbosity minimal /p:Platform=x64 /p:RuntimeIdentifier=win-x64
if ($LASTEXITCODE -ne 0) { throw "Failed building x64 project" }

$mspdbArg = if ($mspdbcmfPath) { "/p:MsPdbCmfExeFullpath=$mspdbcmfPath" } else { "" }
dotnet msbuild 'System Security Studio.csproj' /t:Publish /p:Configuration=Release /p:RuntimeIdentifier=win-x64 /p:AppxPackageDir="MSIXOutputX64\" /p:GenerateAppxPackageOnBuild=true /p:Platform=x64 -v:minimal $mspdbArg
if ($LASTEXITCODE -ne 0) { throw "Failed packaging x64 project" }
#endregion

#region --- Create Certificate and Sign ---
Write-Host "`nCreating certificate and signing packages..." -ForegroundColor Magenta

$Cert = New-SelfSignedCertificate -Type Custom -Subject $PackagePublisher `
    -KeyUsage DigitalSignature -FriendlyName 'OFFSECHQ Code Signing' `
    -CertStoreLocation 'Cert:\CurrentUser\My' `
    -TextExtension @('2.5.29.37={text}1.3.6.1.5.5.7.3.3', '2.5.29.19={text}')

Write-Host "  Certificate thumbprint: $($Cert.Thumbprint)" -ForegroundColor Green

# Find MSIX files
$x64Msix = Get-ChildItem -Path '.\MSIXOutputX64' -Recurse -Filter '*.msix' | Select-Object -First 1
& $SignToolPath sign /fd $PackageHashAlgo /sha1 $Cert.Thumbprint /td $PackageHashAlgo /tr "http://timestamp.digicert.com" $x64Msix.FullName
if ($LASTEXITCODE -ne 0) { throw "Failed signing x64 MSIX" }

# Create bundle output
$null = New-Item -Path '.\MSIXBundleOutput' -ItemType Directory -Force
Copy-Item -Path $x64Msix.FullName -Destination '.\MSIXBundleOutput\' -Force

# Extract version and create bundle
$version = [regex]::Match($x64Msix.Name, '_(\d+\.\d+\.\d+\.\d+)_').Groups[1].Value
$bundleName = "System Security Studio_$version.msixbundle"
$bundlePath = ".\MSIXBundleOutput\$bundleName"

& $MakeAppxPath bundle /d '.\MSIXBundleOutput' /p $bundlePath /o /v
if ($LASTEXITCODE -ne 0) { throw "Failed creating MSIX bundle" }

# Export certificate
$certPath = ".\MSIXBundleOutput\OFFSECHQ_CodeSigning.cer"
Export-Certificate -Cert $Cert -FilePath $certPath -Type CERT

# Sign bundle
& $SignToolPath sign /fd $PackageHashAlgo /sha1 $Cert.Thumbprint /td $PackageHashAlgo /tr "http://timestamp.digicert.com" $bundlePath
if ($LASTEXITCODE -ne 0) { throw "Failed signing MSIX bundle" }
#endregion

#region --- Install (optional) ---
if ($Install) {
    Write-Host "`nInstalling package..." -ForegroundColor Magenta
    
    # Trust certificate
    $store = [System.Security.Cryptography.X509Certificates.X509Store]::new('TrustedPeople', 'LocalMachine')
    $store.Open('ReadWrite')
    $store.Add($Cert)
    $store.Close()
    
    Add-AppPackage -Path $bundlePath -AllowUnsigned -ForceTargetApplicationShutdown
    Write-Host "  Package installed successfully!" -ForegroundColor Green
}
#endregion

#region --- Summary ---
$Stopwatch.Stop()

Write-Host "`n========================================" -ForegroundColor Cyan
Write-Host "  BUILD COMPLETE" -ForegroundColor Green
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "  Bundle: $bundlePath" -ForegroundColor White
Write-Host "  Certificate: $certPath" -ForegroundColor White
Write-Host "  Build Time: $($Stopwatch.Elapsed.ToString('g'))" -ForegroundColor White
Write-Host "========================================`n" -ForegroundColor Cyan
#endregion
