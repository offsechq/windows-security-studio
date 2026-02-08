<div align="center">

# Windows Security Studio

[![License: MIT](https://img.shields.io/badge/License-MIT-blue.svg)](LICENSE)
[![Build ACS](https://github.com/OFFSECHQ/windows-security-studio/actions/workflows/Build%20App%20Control%20Studio%20MSIX%20Package.yml/badge.svg)](https://github.com/OFFSECHQ/windows-security-studio/actions/workflows/Build%20App%20Control%20Studio%20MSIX%20Package.yml)
[![Build SSS](https://github.com/OFFSECHQ/windows-security-studio/actions/workflows/Build%20System%20Security%20Studio%20MSIX%20Package.yml/badge.svg)](https://github.com/OFFSECHQ/windows-security-studio/actions/workflows/Build%20System%20Security%20Studio%20MSIX%20Package.yml)

Native Windows applications for application control and system hardening — built entirely on official Microsoft security features.

[Documentation](https://github.com/OFFSECHQ/windows-security-studio/wiki) · [Releases](https://github.com/OFFSECHQ/windows-security-studio/releases)

</div>

---

## What Is This

Two WinUI 3 desktop apps that harden Windows using only built-in, documented Microsoft security technologies. No third-party drivers, no undocumented registry hacks, no external dependencies.

Both are compiled with **Native AOT** (ahead-of-time compilation) and distributed as self-contained **MSIX** packages.

> **Fork notice** — This project is a maintained fork of [HotCakeX/Harden-Windows-Security](https://github.com/HotCakeX/Harden-Windows-Security) by Violet Hansen. See [License](#license).

---

## App Control Studio

Manages **Windows Defender Application Control (WDAC)** — the zero-trust application allowlisting technology built into Windows.

- Create base, supplemental, and deny policies from file/folder scans or event logs
- Edit CI policy XML visually — rule options, signers, file rules
- Simulate policy enforcement against files before deployment
- Deploy and remove policies locally or via Microsoft Intune
- Build policies from Microsoft Defender for Endpoint Advanced Hunting data
- View file certificates, generate code-signing certs, compute CI hashes
- Merge and validate policies

[Documentation →](https://github.com/OFFSECHQ/windows-security-studio/wiki/App-Control-Studio)

## System Security Studio

Applies, verifies, and manages security configurations across Windows.

| Area | Coverage |
|---|---|
| **Security Baselines** | Microsoft security baselines, Microsoft 365 Apps hardening |
| **Defender & ASR** | Real-time protection, cloud analysis, attack surface reduction rules, exploit mitigations |
| **Encryption** | BitLocker full-volume encryption — configuration and compliance verification |
| **Device Guard** | Credential Guard, VBS, HVCI, kernel-mode integrity |
| **Network** | TLS cipher suite enforcement, Windows Firewall rules, LOLBin blocking, country-based IP blocking |
| **System** | UAC elevation, lock screen hardening, audit policies, certificate validation |
| **Management** | Group Policy editor, Intune/CSP policy inspection, optional features management |

[Documentation →](https://github.com/OFFSECHQ/windows-security-studio/wiki/System-Security-Studio)

---

## Install

### From GitHub Releases

1. Download the latest **Install Kit** `.zip` from [Releases](https://github.com/OFFSECHQ/windows-security-studio/releases)
2. Extract and run `Install.cmd` — it auto-elevates, imports the signing certificate, and installs the package

The apps auto-update: they check GitHub for new versions and can update in-place.

### Build from Source

Requirements: Visual Studio 2022 17.12+, .NET 10 SDK, Windows App SDK, Rust nightly toolchain (App Control Studio only).

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
| **Compilation** | Native AOT, full trimming, Control Flow Guard, CET Shadow Stack |
| **Packaging** | MSIX / MSIXBundle |
| **CI/CD** | GitHub Actions — builds, signs, creates releases, and opens version-bump PRs |
| **Platform** | Windows 10 22H2+ (build 22621), x64 |

---

## License

[MIT](LICENSE)

Original work © 2023 [Violet Hansen](https://github.com/HotCakeX) · Fork modifications © 2026 OFFSECHQ
