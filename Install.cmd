@echo off
:: Windows Security Studio Installer
:: Double-click to install, or run from command line
:: This script auto-elevates to admin and bypasses PowerShell execution policy

setlocal
cd /d "%~dp0"

:: Check if running as admin
net session >nul 2>&1
if %errorlevel% neq 0 goto :elevate
goto :continue

:elevate
echo Requesting Administrator privileges...
set "SCRIPT_DIR=%~dp0"
set "SCRIPT_PATH=%~f0"
powershell -Command "Start-Process cmd -ArgumentList '/c cd /d \"%SCRIPT_DIR%\" && \"%SCRIPT_PATH%\"' -Verb RunAs"
exit /b

:continue

echo.
echo === Windows Security Studio Installer ===
echo.

:: Run PowerShell with the install logic
powershell.exe -NoProfile -ExecutionPolicy Bypass -Command ^
    "$ErrorActionPreference = 'Stop'; " ^
    "$ScriptPath = '%~dp0'; " ^
    "" ^
    "$CertPath = Join-Path $ScriptPath 'OFFSECHQ_CodeSigning.cer'; " ^
    "if (Test-Path $CertPath) { " ^
    "    Write-Host 'Installing Code Signing Certificate...' -ForegroundColor Yellow; " ^
    "    try { " ^
    "        Import-Certificate -FilePath $CertPath -CertStoreLocation Cert:\LocalMachine\TrustedPeople | Out-Null; " ^
    "        Write-Host 'Certificate installed successfully.' -ForegroundColor Green; " ^
    "    } catch { " ^
    "        Write-Error \"Failed to install certificate: $_\"; " ^
    "        Read-Host 'Press Enter to exit...'; " ^
    "        exit 1; " ^
    "    } " ^
    "} else { " ^
    "    Write-Warning 'Certificate not found. Attempting installation anyway...'; " ^
    "}; " ^
    "" ^
    "$BundlePath = Get-ChildItem -Path $ScriptPath -Filter '*.msixbundle' | Select-Object -First 1; " ^
    "if ($null -eq $BundlePath) { " ^
    "    Write-Error 'No .msixbundle file found in script directory.'; " ^
    "    Read-Host 'Press Enter to exit...'; " ^
    "    exit 1; " ^
    "}; " ^
    "" ^
    "Write-Host \"Found package: $($BundlePath.Name)\" -ForegroundColor Cyan; " ^
    "Write-Host 'Installing...' -ForegroundColor Yellow; " ^
    "" ^
    "try { " ^
    "    Add-AppxPackage -Path $BundlePath.FullName -ForceUpdateFromAnyVersion; " ^
    "    Write-Host ''; " ^
    "    Write-Host 'Installation Successful! Launch the app from Start Menu.' -ForegroundColor Green; " ^
    "} catch { " ^
    "    Write-Host 'Retrying with ForceApplicationShutdown...' -ForegroundColor Gray; " ^
    "    try { " ^
    "        Add-AppxPackage -Path $BundlePath.FullName -ForceUpdateFromAnyVersion -ForceTargetApplicationShutdown; " ^
    "        Write-Host 'Installation Successful!' -ForegroundColor Green; " ^
    "    } catch { " ^
    "        Write-Error \"Installation Failed: $_\"; " ^
    "    } " ^
    "}"

echo.
pause
