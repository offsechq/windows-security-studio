<div align="center">

# Windows Security Studio

**Enterprise-grade Windows security hardening using official Microsoft methods.**

Harden personal and enterprise Windows devices against advanced threats — no third-party security software required.

[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)
[![Build App Control Studio](https://github.com/OFFSECHQ/windows-security-studio/actions/workflows/Build%20App%20Control%20Studio%20MSIX%20Package.yml/badge.svg)](https://github.com/OFFSECHQ/windows-security-studio/actions/workflows/Build%20App%20Control%20Studio%20MSIX%20Package.yml)
[![Build System Security Studio](https://github.com/OFFSECHQ/windows-security-studio/actions/workflows/Build%20System%20Security%20Studio%20MSIX%20Package.yml/badge.svg)](https://github.com/OFFSECHQ/windows-security-studio/actions/workflows/Build%20System%20Security%20Studio%20MSIX%20Package.yml)

[Documentation](https://github.com/OFFSECHQ/windows-security-studio/wiki) · [Report a Bug](https://github.com/OFFSECHQ/windows-security-studio/issues) · [Releases](https://github.com/OFFSECHQ/windows-security-studio/releases)

</div>

---

## Overview

Windows Security Studio is a suite of native WinUI 3 desktop applications that leverage built-in Windows security features to bring your system to a maximum security state. Both apps are compiled with **Native AOT** for fast startup and minimal footprint, and distributed as signed **MSIX** packages.

> This is a maintained fork of [HotCakeX/Harden-Windows-Security](https://github.com/HotCakeX/Harden-Windows-Security) by **OFFSECHQ**. A weekly GitHub Actions workflow monitors upstream changes and creates issues for manual review before merging.

---

## Applications

### App Control Studio

A modern interface for managing **Windows Defender Application Control (WDAC)** — the zero-trust application allowlisting technology built into Windows.

| Capability | Description |
|---|---|
| **Policy Creation** | Build base, supplemental, and deny policies from file/folder scans or event logs |
| **Policy Editor** | Visually inspect and modify CI policy XML, configure rule options |
| **Simulation** | Test policies against files before deployment using hash, certificate, and signer analysis |
| **Deployment** | Deploy and remove policies locally or via Microsoft Intune |
| **MDE Integration** | Create policies from Microsoft Defender for Endpoint Advanced Hunting data |
| **Certificate Tools** | View file certificates, generate code-signing certificates, compute CI hashes |
| **Policy Merge & Validation** | Merge multiple policies and validate policy XML integrity |

[App Control Studio Documentation →](https://github.com/OFFSECHQ/windows-security-studio/wiki/App-Control-Studio)

### System Security Studio

A comprehensive system hardening utility that applies, verifies, and manages security configurations across 19 categories of Windows security controls.

| Category | What It Covers |
|---|---|
| **Microsoft Security Baselines** | Apply and override Microsoft-recommended security baselines |
| **Microsoft 365 Apps Baseline** | Harden Office application security settings |
| **Microsoft Defender** | Configure real-time protection, cloud analysis, and ASR rules |
| **Attack Surface Reduction** | Enable exploit mitigations and reduce the OS attack surface |
| **BitLocker** | Full-volume encryption configuration and compliance verification |
| **Device Guard** | Credential Guard, VBS, HVCI, and kernel-mode integrity |
| **TLS & Networking** | Enforce modern cipher suites, disable legacy protocols |
| **Windows Firewall** | Block LOLBins, apply country-based IP blocking |
| **UAC & Lock Screen** | Elevate User Account Control and lock-screen hardening |
| **Certificate Checking** | Validate certificate chain integrity |
| **Audit Policies** | Configure advanced security audit logging |
| **Optional Windows Features** | Remove unnecessary OS components and bloatware |
| **Intune / CSP** | Retrieve and inspect MDM-applied policies |

[System Security Studio Documentation →](https://github.com/OFFSECHQ/windows-security-studio/wiki/System-Security-Studio)

---

## Installation

### From a Release

1. Download the latest installation kit `.zip` from [Releases](https://github.com/OFFSECHQ/windows-security-studio/releases).
2. Extract the archive — it contains the `.msixbundle`, code-signing certificate, and `Install.bat`.
3. Run `Install.bat` — it auto-elevates, installs the certificate, and sideloads the package.

### Build from Source

Both apps target **.NET 10** with **Native AOT** and **WinUI 3** (Windows App SDK). Rust interop is used in App Control Studio.

```
# App Control Studio
cd "App Control Studio"
.\Build-AppControlStudio.ps1

# System Security Studio
cd "System Security Studio"
.\Build-SystemSecurityStudio.ps1
```

**Prerequisites**: Visual Studio 2022 17.12+, .NET 10 SDK, Windows App SDK, Rust toolchain (for App Control Studio).

---

## Design Principles

| Principle | Detail |
|---|---|
| **Official Methods Only** | Exclusively uses documented, supported Microsoft security features — no undocumented registry hacks or third-party drivers |
| **Native AOT** | Both apps are fully AOT-compiled with Control Flow Guard and CET Shadow Stack for maximum runtime security |
| **Defense in Depth** | Layers ASR, Exploit Protection, App Control, Device Guard, BitLocker, Firewall, and more into a unified workflow |
| **Transparent & Verifiable** | Open source. MSIX packages are built via GitHub Actions with full build logs and reproducible pipelines |
| **Self-Contained** | MSIX bundles include the .NET runtime and Windows App SDK — no external dependencies to install |

---

## Security Recommendations

For maximum protection alongside these tools:

- **Official Media** — Install Windows from official Microsoft sources only. Never use modified ISOs.
- **Hardware** — Prefer Secured-Core PCs with TPM 2.0 and DFCI support.
- **Accounts** — Use standard user accounts day-to-day; authenticate with Microsoft Entra ID or Microsoft Accounts with MFA / Passkeys.
- **Network** — Enable DNS over HTTPS (DoH); avoid unnecessary VPN services.
- **Browser** — Use Microsoft Edge for hardware-enforced stack protection and SmartScreen integration.

[Full security guidance →](https://github.com/OFFSECHQ/windows-security-studio/wiki)

---

## Tech Stack

| Layer | Technology |
|---|---|
| UI Framework | WinUI 3 (Windows App SDK) |
| Language | C# (.NET 10, preview language features) |
| Compilation | Native AOT with full trimming |
| Interop | Rust (App Control Studio), C++ |
| Packaging | MSIX / MSIXBundle |
| Platform | Windows 10 22H2+ (build 22621), x64 |

---

## License

MIT — see [LICENSE](LICENSE). Based on original work by [Violet Hansen](https://github.com/HotCakeX).
