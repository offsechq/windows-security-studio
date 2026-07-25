<div align="center">

# Windows Security Studio

[![License: MIT](https://img.shields.io/badge/License-MIT-blue.svg)](LICENSE)
[![Build WSS](https://github.com/OFFSECHQ/windows-security-studio/actions/workflows/Build%20Windows%20Security%20Studio%20MSIX%20Package.yml/badge.svg)](https://github.com/OFFSECHQ/windows-security-studio/actions/workflows/Build%20Windows%20Security%20Studio%20MSIX%20Package.yml)

An open-source, Native AOT Windows application for applying, verifying, and managing Microsoft security controls.

[Documentation](https://github.com/OFFSECHQ/windows-security-studio/wiki) · [Releases](https://github.com/OFFSECHQ/windows-security-studio/releases)

</div>

## Features

- Microsoft Security Baselines and Microsoft 365 Apps hardening
- Microsoft Defender, attack-surface reduction, and exploit protection
- BitLocker, Device Guard, Credential Guard, VBS, and HVCI
- Windows Firewall, networking, TLS, UAC, audit policy, and Windows Update
- Intune/CSP inspection, system-state reporting, and command-line automation

> [!CAUTION]
> These controls can cause lockouts, compatibility problems, or system instability when applied incorrectly. Test changes safely and keep recovery options available.

## Install

1. Download the latest **Windows Security Studio Install Kit** ZIP from [GitHub Releases](https://github.com/OFFSECHQ/windows-security-studio/releases/latest).
2. Extract the ZIP.
3. Run `Install.cmd` and approve the administrator prompt.

The install kit contains the signed MSIX bundle, signing certificate, and installer.

## Build from Source

Build on Windows 11 or Windows Server 2025 with .NET 10, Rust nightly, and the Visual Studio C++ and Windows application build tools:

```powershell
cd "Windows Security Studio"
.\Build-WindowsSecurityStudio.ps1
```

## Documentation

See the [Windows Security Studio wiki](https://github.com/OFFSECHQ/windows-security-studio/wiki) for usage, security controls, command-line options, and technical details.

## License

Windows Security Studio is licensed under the [MIT License](LICENSE).
