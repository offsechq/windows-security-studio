<div align="center">

# Windows Security Studio

[![License: MIT](https://img.shields.io/badge/License-MIT-blue.svg)](LICENSE)
[![Build ACS](https://github.com/OFFSECHQ/windows-security-studio/actions/workflows/Build%20App%20Control%20Studio%20MSIX%20Package.yml/badge.svg)](https://github.com/OFFSECHQ/windows-security-studio/actions/workflows/Build%20App%20Control%20Studio%20MSIX%20Package.yml)
[![Build SSS](https://github.com/OFFSECHQ/windows-security-studio/actions/workflows/Build%20System%20Security%20Studio%20MSIX%20Package.yml/badge.svg)](https://github.com/OFFSECHQ/windows-security-studio/actions/workflows/Build%20System%20Security%20Studio%20MSIX%20Package.yml)

Native Windows applications for application control and system hardening, built entirely on official Microsoft security features.

[Documentation](https://github.com/OFFSECHQ/windows-security-studio/wiki) · [Releases](https://github.com/OFFSECHQ/windows-security-studio/releases)

</div>

---

## Overview

Windows Security Studio includes two WinUI 3 desktop apps that harden Windows using built-in, documented Microsoft security technologies.

No third-party drivers. No undocumented registry hacks. No external security engines.

Both apps are compiled with **Native AOT** and distributed as self-contained **MSIX** packages.

---

## Important Safety Notice

Some features in **App Control Studio** and **System Security Studio** are advanced hardening controls that can cause system instability, lockouts, or reduced functionality if applied incorrectly.

Use these tools only if you understand the settings you are changing, test changes in a safe environment first, and keep recovery options available.

By using this project, you accept full responsibility for any impact. Use at your own risk.

---

## App Control Studio

Manages **Windows Defender Application Control (WDAC)**, the allowlisting technology built into Windows.

- Build base, supplemental, and deny policies from scans or event logs
- Visually edit CI policy XML (rule options, signers, and file rules)
- Simulate enforcement before deployment
- Deploy and remove policies locally or through Intune
- Merge and validate policies

[Documentation →](https://github.com/OFFSECHQ/windows-security-studio/wiki/App-Control-Studio)

## System Security Studio

Applies, verifies, and manages hardening settings across Windows.

- Security baselines and Microsoft 365 Apps hardening
- Defender and ASR management, including exploit mitigations
- BitLocker configuration and compliance verification
- Device Guard controls (Credential Guard, VBS, HVCI)
- Network and firewall hardening, including TLS controls and LOLBin blocking
- System and management controls across UAC, audit policy, Group Policy, and Intune/CSP inspection

[Documentation →](https://github.com/OFFSECHQ/windows-security-studio/wiki/System-Security-Studio)

---

## Install

### From GitHub Releases

1. Download the latest **Install Kit** `.zip` from [Releases](https://github.com/OFFSECHQ/windows-security-studio/releases)
2. Extract and run `Install.cmd` (auto-elevates, imports the signing certificate, and installs the package)

Both apps can check GitHub for updates and update in place.

### Build from Source

Requirements:
- Visual Studio 2022 17.12+
- .NET 10 SDK
- Windows App SDK
- Rust nightly toolchain (App Control Studio only)

```powershell
# App Control Studio
cd "App Control Studio"
.\Build-AppControlStudio.ps1

# System Security Studio
cd "System Security Studio"
.\Build-SystemSecurityStudio.ps1
```

---

## Tech Stack

| | |
|---|---|
| **UI** | WinUI 3 (Windows App SDK) |
| **Languages** | C# (.NET 10), Rust, C++ |
| **Compilation** | Native AOT, trimming, Control Flow Guard, CET Shadow Stack |
| **Packaging** | MSIX / MSIXBundle |
| **CI/CD** | GitHub Actions |
| **Platform** | Windows 10 22H2+ (build 19045+) / Windows 11 22H2+ (build 22621+), x64 |

---

## License

[MIT](LICENSE)
