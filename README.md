<div align="center">

# Windows Security Studio

[![License: MIT](https://img.shields.io/badge/License-MIT-blue.svg)](LICENSE)
[![Build WSS](https://github.com/OFFSECHQ/windows-security-studio/actions/workflows/Build%20Windows%20Security%20Studio%20MSIX%20Package.yml/badge.svg)](https://github.com/OFFSECHQ/windows-security-studio/actions/workflows/Build%20Windows%20Security%20Studio%20MSIX%20Package.yml)

A native Windows application for applying, verifying, and managing Microsoft security controls.

[Documentation](https://github.com/OFFSECHQ/windows-security-studio/wiki) · [Releases](https://github.com/OFFSECHQ/windows-security-studio/releases)

</div>

---

## Overview

Windows Security Studio (WSS) hardens Windows using built-in, documented Microsoft security technologies. It does not install third-party drivers or use an external security engine.

WSS includes:

- Microsoft Security Baselines and Microsoft 365 Apps hardening
- Microsoft Defender, attack-surface reduction, and exploit-protection management
- BitLocker configuration and compliance verification
- Device Guard controls, including Credential Guard, VBS, and HVCI
- Network, Windows Firewall, and TLS hardening
- UAC, audit policy, Group Policy, Windows Update, and Intune/CSP inspection
- Security reporting, policy import/export, and command-line automation

The application is built with Native AOT and distributed as a self-contained MSIX package.

## Important Safety Notice

WSS exposes advanced security controls that can cause system instability, lockouts, or reduced functionality when applied incorrectly. Test changes in a safe environment, understand the settings being changed, and keep recovery options available.

By using this project, you accept responsibility for its impact. Use it at your own risk.

## Install

### GitHub Releases

1. Download the latest **Windows Security Studio Install Kit** `.zip` from [Releases](https://github.com/OFFSECHQ/windows-security-studio/releases).
2. Extract the archive.
3. Run `Install.cmd`. The installer elevates, trusts the repository's package-signing certificate, and installs or updates the MSIX bundle.

### Microsoft Store

Install from the [Microsoft Store](https://apps.microsoft.com/detail/9p7ggfl7dx57), or run:

```powershell
winget install --id 9p7ggfl7dx57 --exact --accept-package-agreements --accept-source-agreements --force --source msstore
```

## Build from Source

Requirements:

- Windows 11 or Windows Server 2025
- Visual Studio 2022 with Desktop development with C++ and Windows application development workloads
- .NET 10 SDK
- Rust nightly with the `x86_64-pc-windows-msvc` target

```powershell
cd "Windows Security Studio"
.\Build-WindowsSecurityStudio.ps1
```

The build script compiles the Rust interoperability library, native COM helper, DISM helper, privileged relay service, and the WSS MSIX package.

## Repository Layout

| Path | Purpose |
|---|---|
| `Windows Security Studio/` | WSS application and packaging project |
| `Windows Security Studio/CommonCore/` | Shared application infrastructure now owned by WSS |
| `Windows Security Studio/Components/` | Rust, C++, DISM, and privileged-service helper projects |
| `Wiki posts/Windows Security Studio/` | WSS-only wiki sources |
| `.github/workflows/Build Windows Security Studio MSIX Package.yml` | Reproducible Windows packaging and release workflow |

## Tech Stack

| Area | Technology |
|---|---|
| UI | WinUI 3 / Windows App SDK |
| Languages | C# (.NET 10), Rust, C++ |
| Compilation | Native AOT, trimming, Control Flow Guard, CET Shadow Stack |
| Packaging | MSIX / MSIXBundle |
| Automation | GitHub Actions and Dependabot |
| Platform | Windows, x64 |

## Documentation

Start with the [Windows Security Studio wiki](https://github.com/OFFSECHQ/windows-security-studio/wiki/Windows-Security-Studio). The repository intentionally carries only WSS documentation.

## License

[MIT](LICENSE)
